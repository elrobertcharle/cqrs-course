using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Api.Producer.Handlers;
using Post.Cmd.Api.Producer.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Post.Cmd.Api.Producer
{
    public class KafkaOutboxListener : BackgroundService
    {
        private readonly ILogger<KafkaOutboxListener> _logger;
        private readonly KafkaOptions _kafkaConfig;
        private IServiceScopeFactory _serviceScopeFactory;

        public KafkaOutboxListener(ILogger<KafkaOutboxListener> logger, IServiceScopeFactory ssf, IOptions<KafkaOptions> kafkaConfig, IValidator<KafkaOptions> kafkaConfigValidator)
        {
            _logger = logger;
            var vr = kafkaConfigValidator.Validate(kafkaConfig.Value);
            if (!vr.IsValid)
                throw new ConfigurationException($"KafkaConfig validation failed: {vr.ToString(", ")}");
            _kafkaConfig = kafkaConfig.Value;
            _serviceScopeFactory = ssf;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(_kafkaConfig.ConsumerConfig).Build();
            consumer.Subscribe(_kafkaConfig.IncomingTopic);

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        ConsumeResult<Ignore, string>? cr = null;
                        try
                        {
                            cr = consumer.Consume(ct);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }

                        if (cr?.Message?.Value == null)
                        {
                            _logger.LogWarning("Null message received from Kafka. Topic: {IncomingTopic}", _kafkaConfig.IncomingTopic);
                            continue;
                        }

                        var outboxEvent = JsonSerializer.Deserialize<NewOutboxCreatedEvent>(cr.Message.Value);
                        if (outboxEvent == null)
                        {
                            _logger.LogError("The message could not be deserialized to type {OutboxEventType}. Event: {Event}", nameof(NewOutboxCreatedEvent), cr.Message.Value);
                            continue;
                        }
                        using var scope = _serviceScopeFactory.CreateScope();
                        var outboxEventHandler = scope.ServiceProvider.GetRequiredService<IOutboxEventHandler>();
                        await outboxEventHandler.HandleAsync(outboxEvent, ct);

                        consumer.Commit(cr);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled error in Kafka processing loop.");
                    }
                }
            }
            finally
            {
                try
                {
                    consumer.Close(); // Leaves the group cleanly and commits offsets
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Exception during Kafka consumer close.");
                }
            }
        }
    }
}
