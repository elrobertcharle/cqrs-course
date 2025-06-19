using Microsoft.EntityFrameworkCore;
using Post.Common.Events;
using Post.Query.Api.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Consumer.Handlers
{
    public class PostMessageUpdatedEventHandler : IEventHandler<PostMessageUpdatedEvent>
    {
        private readonly DatabaseContext _dbContext;

        public PostMessageUpdatedEventHandler(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(PostMessageUpdatedEvent @event, CancellationToken ct)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == @event.PostId, ct);
            if (post == null)
                throw new InvalidOperationException($"The post with Id={@event.PostId} does not exist.");

            post.Message = @event.Message;
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
