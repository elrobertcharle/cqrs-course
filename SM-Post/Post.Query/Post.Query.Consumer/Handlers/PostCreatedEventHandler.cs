using Post.Common.Events;
using Post.Query.Api.Database;
using Post.Query.Api.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Consumer.Handlers
{
    public class PostCreatedEventHandler : IEventHandler<PostCreatedEvent>
    {
        private readonly ILogger<PostCreatedEventHandler> _logger;
        private readonly DatabaseContext _dbContext;

        public PostCreatedEventHandler(ILogger<PostCreatedEventHandler> logger, DatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task HandleAsync(PostCreatedEvent @event, CancellationToken ct)
        {
            var post = new PostEntity
            {
                Id = @event.PostId,
                Author = @event.Author,
                CreatedDate = @event.DatePosted,
                Message = @event.Message
            };

            _dbContext.Add(post);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
