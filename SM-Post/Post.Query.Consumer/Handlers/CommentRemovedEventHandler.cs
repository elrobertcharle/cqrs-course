using Post.Common.Events;
using Post.Query.Api.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Consumer.Handlers
{
    public class CommentRemovedEventHandler : IEventHandler<CommentRemovedEvent>
    {
        private readonly DatabaseContext _dbContext;

        public CommentRemovedEventHandler(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(CommentRemovedEvent @event, CancellationToken ct)
        {
            var comment = await _dbContext.Comments.FindAsync(@event.CommentId);
            if (comment == null)
                return;
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
