using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IEventHandler _eventHandler;

        public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
        {
            _config = config.Value;
            _eventHandler = eventHandler;
        }

        public void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config).SetKeyDeserializer(Deserializers.Utf8).SetValueDeserializer(Deserializers.Utf8).Build();
            consumer.Subscribe(topic);
            while (true)
            {
                var consumeResult = consumer.Consume();
                if (consumeResult?.Message == null)
                    continue;

                var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
                if (@event == null)
                    throw new InvalidOperationException("Could not deserialize the event.");

                var handlerMethod = _eventHandler.GetType().GetMethod("On", [@event.GetType()]);

                if (handlerMethod == null)
                    throw new InvalidOperationException($"Method On was not found with a parameter of type {@event.GetType()}");

                handlerMethod.Invoke(_eventHandler, [@event]);
                consumer.Commit(consumeResult);
            }
        }
    }
}
