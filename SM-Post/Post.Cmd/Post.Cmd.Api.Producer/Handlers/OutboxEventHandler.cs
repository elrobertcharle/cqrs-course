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
        Task HandleAsync(NewOutboxCreatedEvent @event, CancellationToken ct);
    }

    public class OutboxEventHandler : IOutboxEventHandler
    {
        private readonly ILogger<OutboxPollingWorker> _logger;
        private readonly KafkaOptions _kafkaConfig;
        private readonly IMongoCollection<OutboxMessage> _outboxMessagesCollection;

        public OutboxEventHandler(ILogger<OutboxPollingWorker> logger, IOptions<KafkaOptions> kafkaConfig, IValidator<KafkaOptions> kafkaConfigValidator, IOptions<MongoDbOptions> mongoDbConfig)
        {
            _logger = logger;
            var vr = kafkaConfigValidator.Validate(kafkaConfig.Value);
            if (!vr.IsValid)
                throw new ConfigurationException($"KafkaConfig validation failed: {vr.ToString(", ")}");
            _kafkaConfig = kafkaConfig.Value;

            var mongoClient = new MongoClient(mongoDbConfig.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbConfig.Value.Database);
            _outboxMessagesCollection = mongoDatabase.GetCollection<OutboxMessage>("outboxMessages");
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

            var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };

            try
            {
                var kafkaMessage = new Message<string, string>
                {
                    Key = outboxMessage.KafkaKey,
                    Value = outboxMessage.Payload
                };

                await producer.ProduceAsync(outboxMessage.KafkaTopic ?? _kafkaConfig.OutgoingTopic, kafkaMessage, ct);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event: {OutboxMessageId}", outboxMessage.Id);
            }

        }
    }
}
