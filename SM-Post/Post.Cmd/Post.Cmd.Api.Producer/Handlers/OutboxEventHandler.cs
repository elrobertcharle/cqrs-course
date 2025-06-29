using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Messages;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Api.Producer.Options;
using Post.Common.Events.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Post.Cmd.Api.Producer.Handlers
{
    public interface IOutboxEventHandler
    {
        /// <summary>
        /// Executed when a NewOutboxCreatedEvent arrives. NewOutboxCreatedEvent events are sent by the service that inserts into the Outbox collection.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task HandleAsync(NewOutboxCreatedEvent @event, CancellationToken ct);

        /// <summary>
        /// Executed from time to time. It is a fallback for cases where HandleAsync could not be called due to communication problems with the event queue. 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task SendPendingAsync(CancellationToken ct);
    }

    public class OutboxEventHandler : IOutboxEventHandler
    {
        private readonly ILogger<OutboxPollingWorker> _logger;
        private readonly KafkaOptions _kafkaConfig;
        private readonly IMongoCollection<OutboxMessage> _outboxMessagesCollection;
        private readonly OutboxPollingWorkerOptions _outboxPollingWorkerConfig;

        public OutboxEventHandler(ILogger<OutboxPollingWorker> logger, IOptions<KafkaOptions> kafkaConfig, IValidator<KafkaOptions> kafkaConfigValidator, IOptions<MongoDbOptions> mongoDbConfig, IOptions<OutboxPollingWorkerOptions> outboxPollingWorkerConfig)
        {
            _logger = logger;
            var vr = kafkaConfigValidator.Validate(kafkaConfig.Value);
            if (!vr.IsValid)
                throw new ConfigurationException($"KafkaConfig validation failed: {vr.ToString(", ")}");
            _kafkaConfig = kafkaConfig.Value;

            var mongoClient = new MongoClient(mongoDbConfig.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbConfig.Value.Database);
            _outboxMessagesCollection = mongoDatabase.GetCollection<OutboxMessage>("outboxMessages");
            _outboxPollingWorkerConfig = outboxPollingWorkerConfig.Value;
        }

        public async Task HandleAsync(NewOutboxCreatedEvent newOutboxEventCreated, CancellationToken ct)
        {
            var producer = new ProducerBuilder<string, string>(_kafkaConfig.ProducerConfig).Build();

            var filterBuilder = Builders<OutboxMessage>.Filter;
            var filter = filterBuilder.Eq(m => m.Id, newOutboxEventCreated.TargetEventId);

            var cursor = await _outboxMessagesCollection.FindAsync(filter, cancellationToken: ct);
            var l = await cursor.ToListAsync(ct).ConfigureAwait(false);
            var outboxMessage = l.FirstOrDefault();

            if (outboxMessage == null)
            {
                _logger.LogError("OutboxMessage not found. {@NewOutboxEventCreated}", newOutboxEventCreated);
                return;
            }

            try
            {
                if (!await PublishEvent(producer, outboxMessage, ct))
                    return;
                await SetOutboxMessageAsPublished(outboxMessage.Id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event: {OutboxMessageId}", outboxMessage.Id);
            }

        }

        public async Task SendPendingAsync(CancellationToken ct)
        {
            var WorkerIndexEnvVarName = "WORKER_INDEX";
            var strWorkerIndex = Environment.GetEnvironmentVariable(WorkerIndexEnvVarName);
            if (strWorkerIndex == null)
                throw new ConfigurationException($"EnvironmentVariable {WorkerIndexEnvVarName} was not set.");
            if (!int.TryParse(strWorkerIndex, out var workerIndex))
                throw new ConfigurationException($"EnvironmentVariable {WorkerIndexEnvVarName} is invalid.");

            var producer = new ProducerBuilder<string, string>(_kafkaConfig.ProducerConfig).Build();

            var filterBuilder = Builders<OutboxMessage>.Filter;
            var filter = filterBuilder.Eq(m => m.IsPublished, false) & filterBuilder.Mod(m => m.KafkaKeyHash, _outboxPollingWorkerConfig.TotalWorkers, workerIndex);
            var sortBuilder = Builders<OutboxMessage>.Sort;
            var sort = sortBuilder.Ascending(m => m.CreatedAt);

            var pendingMessages = await _outboxMessagesCollection.Find(filter).Sort(sort).Limit(1000).ToListAsync(ct).ConfigureAwait(false);

            foreach (var outboxMessage in pendingMessages)
            {
                try
                {
                    if (!await PublishEvent(producer, outboxMessage, ct))
                        break;
                    await SetOutboxMessageAsPublished(outboxMessage.Id, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish event: {OutboxMessageId}", outboxMessage.Id);
                }
            }
        }

        private async Task<bool> PublishEvent(IProducer<string, string> producer, OutboxMessage outboxMessage, CancellationToken ct)
        {
            var kafkaMessage = new Message<string, string>
            {
                Key = outboxMessage.KafkaKey,
                Value = outboxMessage.Payload
            };

            // sent to kafka
            var deliveryResult = await producer.ProduceAsync(outboxMessage.KafkaTopic ?? _kafkaConfig.OutgoingTopic, kafkaMessage, ct);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                _logger.LogError("Unable to persist the message in Kafka. {OutboxMessageId}.", outboxMessage.Id);
                return false;
            }

            return true;
        }

        private async Task SetOutboxMessageAsPublished(Guid outboxMessageId, CancellationToken ct)
        {

            var update = Builders<OutboxMessage>.Update.Set(m => m.IsPublished, true).Set(m => m.PublishedAt, DateTime.UtcNow);

            var idFilter = Builders<OutboxMessage>.Filter.Eq(m => m.Id, outboxMessageId);

            await _outboxMessagesCollection.UpdateOneAsync(idFilter, update, cancellationToken: ct);
        }
    }
}
