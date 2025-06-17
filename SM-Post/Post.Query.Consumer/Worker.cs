using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Options;
using Post.Query.Consumer.Config;
using Post.Query.Consumer.Handlers;
using System.Text.Json;

namespace Post.Query.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly KafkaConfig _kafkaConfig;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory ssf, IOptions<KafkaConfig> kafkaConfig, IValidator<KafkaConfig> kafkaConfigValidator)
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
            consumer.Subscribe(_kafkaConfig.Topic);

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

                        if (cr == null || cr.Message == null)
                        {
                            _logger.LogWarning("Null message received from Kafka. Topic: {KafkaTopic}", _kafkaConfig.Topic);
                            continue;
                        }

                        var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                        var @event = JsonSerializer.Deserialize<BaseEvent>(cr.Message.Value, options);
                        if (@event == null)
                        {
                            _logger.LogWarning("Null event deserialized. Topic: {KafkaTopic}", _kafkaConfig.Topic);
                            continue;
                        }

                        var eventType = @event.GetType();
                        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        using var scope = _serviceScopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetService(handlerType);
                        if (handler == null)
                        {
                            _logger.LogError("Handler not found. Expected: {ExpectedHandlerType}, Event: {EventType}", handlerType, eventType);
                            continue;
                        }

                        var method = handlerType.GetMethod("HandleAsync");
                        if (method == null)
                        {
                            _logger.LogError("HandleAsync method not found on {HandlerType}", handlerType);
                            continue;
                        }

                        var task = (Task?)method.Invoke(handler, [@event, ct]);
                        if (task != null)
                            await task;

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