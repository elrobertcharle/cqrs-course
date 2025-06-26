using CQRS.Core.Messages;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Repositories
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message, IClientSessionHandle session, CancellationToken ct);
        Task<List<OutboxMessage>> GetUnpublishedAsync(CancellationToken ct);
        Task MarkAsPublishedAsync(Guid id, CancellationToken ct);
    }

    public class OutboxRepository : IOutboxRepository
    {
        private readonly IMongoCollection<OutboxMessage> _collection;

        public OutboxRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var database = mongoClient.GetDatabase(config.Value.Database);
            _collection = database.GetCollection<OutboxMessage>("OutboxMessages");
        }

        public async Task AddAsync(OutboxMessage message, IClientSessionHandle session, CancellationToken ct)
        {
            await _collection.InsertOneAsync(session, message, cancellationToken: ct);
        }

        public async Task<List<OutboxMessage>> GetUnpublishedAsync(CancellationToken ct)
        {
            return await _collection.Find(m => !m.IsPublished).ToListAsync(ct);
        }

        public async Task MarkAsPublishedAsync(Guid id, CancellationToken ct)
        {
            var update = Builders<OutboxMessage>.Update
                .Set(x => x.IsPublished, true)
                .Set(x => x.PublishedAt, DateTime.UtcNow);

            await _collection.DeleteOneAsync(m => m.Id == id, ct);
        }
    }

}
