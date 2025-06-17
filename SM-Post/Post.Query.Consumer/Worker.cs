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

            while (!ct.IsCancellationRequested)
            {
                var cr = consumer.Consume(ct);

                var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                var @event = JsonSerializer.Deserialize<BaseEvent>(cr.Message.Value, options);
                if (@event == null)
                {
                    _logger.LogWarning("Null event arrived. {KafkaTopic}", _kafkaConfig.Topic);
                    continue;
                }

                var eventType = @event.GetType();
                var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                using var scope = _serviceScopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetService(handlerType);
                if (handler == null)
                {
                    _logger.LogError("Handler not found. {ExpectedHandlerType}, {EventType}", handlerType, eventType);
                    continue;
                }

                var method = handlerType.GetMethod("HandleAsync");
                if (method == null)
                {
                    _logger.LogError("HandleAsync method not found on {HandlerType}", handlerType);
                    continue;
                }

                try
                {
                    var task = (Task?)method.Invoke(handler, new object[] { @event, ct });
                    if (task != null)
                        await task;
                    consumer.Commit(cr);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error invoking handler for event type {EventType}", eventType);
                }
            }
        }
    }
}
