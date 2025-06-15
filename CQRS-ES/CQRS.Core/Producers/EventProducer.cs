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

        public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
        {
            var producer = new ProducerBuilder<string, string>(_config).Build();

            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(), //roberto: mal, se debe usar una key con sentido
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            };

            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                throw new Exception($"Could not produce {@event.GetType().Name}. Topic={topic}, due to {deliveryResult.Message}");
        }
    }
}
