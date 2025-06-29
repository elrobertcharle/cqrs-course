using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using MongoDB.Driver.Core.Servers;
using Post.Cmd.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore _eventStore;
        private readonly IEventProducer _eventProducer;

        public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
        {
            _eventStore = eventStore;
            _eventProducer = eventProducer;
        }

        public async Task<PostAggregate> GetByIdAsync(Guid aggregateId, CancellationToken ct)
        {
            var aggregate = new PostAggregate();
            var events = await _eventStore.GetEventAsync(aggregateId, ct);

            if (events == null || !events.Any())
                return aggregate;

            aggregate.ReplayEvents(events);
            aggregate.Version = events[^1].Version; // roberto: creo que la version debe ser establecida dentro de replay.
            return aggregate;
        }

        public async Task RepublishEventAsync(CancellationToken ct)
        {
            var aggregateIds = await _eventStore.GetAggregateIdsAsync(ct);
            foreach (var aggregateId in aggregateIds)
            {
                var aggregate = await GetByIdAsync(aggregateId, ct);
                if (!aggregate.Active)
                    continue;

                var events = await _eventStore.GetEventAsync(aggregateId, ct);
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                if (topic == null)
                    throw new InvalidOperationException("Topic has not been set. env var KAFKA_TOPIC.");
                foreach (var @event in events)
                    await _eventProducer.ProduceAsync(topic, aggregateId.ToString("N"), @event);
            }
        }

        public async Task SaveAsync(AggregateRoot aggregate, CancellationToken ct)
        {
            await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version, ct);
            aggregate.MarkChangesAsCommitted();
        }
    }
}
