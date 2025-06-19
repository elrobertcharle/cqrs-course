using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;

        public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
        }

        public async Task<List<Guid>> GetAggregateIdsAsync(CancellationToken ct)
        {
            var events = await _eventStoreRepository.FindAllAsync(ct);
            return events.Select(e => e.AggregateIdentifier).Distinct().ToList();
        }

        public async Task<List<BaseEvent>> GetEventAsync(Guid aggregateId, CancellationToken ct)
        {
            var events = await _eventStoreRepository.FindByAggregateIdAsync(aggregateId, ct);
            if (events == null || !events.Any())
                throw new AggregateNotFoundException(aggregateId);

            return events.OrderBy(e => e.Version).Select(e => e.EventData).ToList();
        }

        public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedLastVersion, CancellationToken ct)
        {
            var storedEvents = await _eventStoreRepository.FindByAggregateIdAsync(aggregateId, ct);
            if (expectedLastVersion != -1 && storedEvents[^1].Version != expectedLastVersion)
                throw new ConcurrencyException();
            var version = expectedLastVersion;
            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.UtcNow,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };

                await _eventStoreRepository.SaveAsync(eventModel, ct);
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                if (topic == null)
                    throw new InvalidOperationException("Topic has not been set. env var KAFKA_TOPIC.");
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
