using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Messages;
using CQRS.Core.Producers;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Repositories;
using System.Text.Json;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IMongoClient _mongoClient;
        private readonly IOutboxRepository _outboxRepository;

        public EventStore(IEventStoreRepository eventStoreRepository, IOutboxRepository outboxRepository, IEventProducer eventProducer, IMongoClient mongoClient)
        {
            _eventStoreRepository = eventStoreRepository;
            _mongoClient = mongoClient;
            _outboxRepository = outboxRepository;
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
            using var session = await _mongoClient.StartSessionAsync(cancellationToken: ct);
            session.StartTransaction();

            try
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

                    await _eventStoreRepository.SaveAsync(eventModel, session, ct);

                    var outboxMessage = new OutboxMessage
                    {
                        AggregateId = aggregateId,
                        AggregateType = nameof(PostAggregate),
                        EventType = eventType,
                        Version = version,
                        Payload = JsonSerializer.Serialize(@event, @event.GetType())
                    };

                    await _outboxRepository.AddAsync(outboxMessage, session, ct);

                    await session.CommitTransactionAsync(ct);
                }
            }
            catch
            {
                await session.AbortTransactionAsync(ct);
                throw;
            }
        }
    }
}
