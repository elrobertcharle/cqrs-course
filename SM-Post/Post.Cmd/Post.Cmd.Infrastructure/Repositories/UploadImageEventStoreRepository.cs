using Microsoft.Extensions.Options;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Repositories
{
    public class UploadImageEventStoreRepository : EventStoreRepository<UploadImageAggregate>, IUploadImageEventStoreRepository
    {
        public UploadImageEventStoreRepository(IOptions<MongoDbConfig> config) : base(config)
        {
        }
    }
}
