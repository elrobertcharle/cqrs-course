using Microsoft.EntityFrameworkCore;
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
    public class CommentUpdatedEventHandler : IEventHandler<CommentUpdatedEvent>
    {
        private readonly DatabaseContext _dbContext;

        public CommentUpdatedEventHandler(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(CommentUpdatedEvent @event, CancellationToken ct)
        {
            var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == @event.CommentId, ct);
            if (comment == null)
                return;

            comment.Comment = @event.CommentText;
            comment.Edited = true;
            comment.UpdatedDate = @event.UpdatedDate;

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
