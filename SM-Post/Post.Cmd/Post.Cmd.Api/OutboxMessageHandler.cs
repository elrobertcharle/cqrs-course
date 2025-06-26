using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Producers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Post.Cmd.Api.Options;
using Post.Cmd.Infrastructure.Repositories;
using Post.Common.Events;
using Post.Common.Events.JsonConverters;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;

namespace Post.Cmd.Api
{
    public interface IOutboxMessageHandler
    {
        Task SendPendingAsync(CancellationToken ct);
    }

    public class OutboxMessageHandler : IOutboxMessageHandler
    {
        private readonly IOutboxRepository _outboxRepository;
        private readonly IEventProducer _producer;
        private readonly ILogger<OutboxWorker> _logger;
        private readonly KafkaProducerOptions _kafkaOptions;

        public OutboxMessageHandler(IEventProducer producer, IOutboxRepository outboxRepository, ILogger<OutboxWorker> logger, IOptions<KafkaProducerOptions> kafkaOptions, IValidator<KafkaProducerOptions> kafkaOptionsValidator)
        {
            _producer = producer;
            _outboxRepository = outboxRepository;
            _kafkaOptions = kafkaOptions.Value;
            _logger = logger;
            var vr = kafkaOptionsValidator.Validate(kafkaOptions.Value);
            if (!vr.IsValid)
                throw new ConfigurationException($"{nameof(KafkaProducerOptions)} validation failed: {vr.ToString(", ")}");

        }

        public async Task SendPendingAsync(CancellationToken ct)
        {
            var topic = _kafkaOptions.Topic;
            var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };

            var outboxMessages = await _outboxRepository.GetUnpublishedAsync(ct);
            foreach (var outboxMessage in outboxMessages)
            {
                try
                {
                    var @event = JsonSerializer.Deserialize<BaseEvent>(outboxMessage.Payload, options);

                    if (@event == null)
                        throw new InvalidOperationException();

                    await _producer.ProduceAsync(topic, @event);



                    await _outboxRepository.MarkAsPublishedAsync(outboxMessage.Id, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish event: {OutboxMessageId}", outboxMessage.Id);
                }
            }
        }
    }
}
