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
    public class CommentAddedEventHandler : IEventHandler<CommentAddedEvent>
    {
        private readonly DatabaseContext _dbContext;

        public CommentAddedEventHandler(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task HandleAsync(CommentAddedEvent @event, CancellationToken ct)
        {
            _dbContext.Comments.Add(new CommentEntity
            {
                PostId = @event.PostId,
                Id = @event.CommentId,
                CreatedDate = @event.CreatedDate,
                Comment = @event.CommentText,
                Username = @event.Username,
                Edited = false,
            });

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
