using Confluent.Kafka;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CQRS.Core.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;

        public EventProducer(IOptions<ProducerConfig> config)
        {
            _config = config.Value;
        }

        public async Task ProduceAsync<T>(string topic, string key, T value) where T : BaseEvent
        {
            var producer = new ProducerBuilder<string, string>(_config).Build();

            var eventMessage = new Message<string, string>
            {
                Key = key,
                Value = JsonSerializer.Serialize(value, value.GetType())
            };

            await ProduceAsync(topic, eventMessage);
        }

        public async Task ProduceAsync<TKey, TValue>(string topic, Message<TKey, TValue> message)
        {
            var producer = new ProducerBuilder<TKey, TValue>(_config).Build();
            var deliveryResult = await producer.ProduceAsync(topic, message);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                throw new Exception($"Could not produce message. Topic={topic}, due to {deliveryResult.Message}.");
        }
    }
}
