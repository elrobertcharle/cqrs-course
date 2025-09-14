using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Repositories
{
    public class PostEventStoreRepository : EventStoreRepository<PostAggregate>, IPostEventStoreRepository
    {
        public PostEventStoreRepository(IOptions<MongoDbConfig> config) : base(config)
        {
        }
    }
}
