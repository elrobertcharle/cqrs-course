using Confluent.Kafka;
using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Producers
{
    public interface IEventProducer
    {
        Task ProduceAsync<T>(string topic, string key, T value) where T : BaseEvent;
        Task ProduceAsync<TKey, TValue>(string topic, Message<TKey, TValue> message);
    }
}
