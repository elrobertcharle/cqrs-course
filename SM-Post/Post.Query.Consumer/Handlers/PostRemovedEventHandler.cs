using Post.Common.Events;
using Post.Query.Api.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Consumer.Handlers
{
    public class PostRemovedEventHandler : IEventHandler<PostRemovedEvent>
    {
        private readonly DatabaseContext _dbContext;

        public PostRemovedEventHandler(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(PostRemovedEvent @event, CancellationToken ct)
        {
            var post = await _dbContext.Posts.FindAsync(@event.PostId);
            if (post == null)
                return;
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
