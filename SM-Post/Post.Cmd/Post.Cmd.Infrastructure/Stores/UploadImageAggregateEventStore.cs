using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Stores
{
    public class UploadImageAggregateEventStore : EventStore<UploadImageAggregate>
    {
        public UploadImageAggregateEventStore(EventStoreRepository<UploadImageAggregate> eventStoreRepository, IOutboxRepository outboxRepository, IEventProducer eventProducer, IMongoClient mongoClient) : base(eventStoreRepository, outboxRepository, eventProducer, mongoClient)
        {
        }
    }
}
