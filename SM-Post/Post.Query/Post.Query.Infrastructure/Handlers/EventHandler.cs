using Microsoft.Extensions.Hosting;
using Post.Common.Events;
using Post.Query.Api.Database.Entities;
using Post.Query.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;

        public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        public async Task On(PostCreatedEvent @event)
        {
            var post = new PostEntity
            {
                Id = @event.PostId,
                Author = @event.Author,
                CreatedDate = @event.DatePosted,
                Message = @event.Message
            };

            await _postRepository.CreateAsync(post);
        }

        public async Task On(PostMessageUpdatedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.PostId); //roberto: no queda claro que el id del evento sea el id de la entidad. I dont like this
            if (post == null)
                throw new InvalidOperationException($"The post with Id={@event.PostId} does not exist.");
            post.Message = @event.Message;
            await _postRepository.UpdateAsync(post);
        }

        public async Task On(PostLikedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.PostId); //roberto: no queda claro que el id del evento sea el id de la entidad. I dont like this
            if (post == null)
                return;
            post.Likes++;
            await _postRepository.UpdateAsync(post);
        }

        public async Task On(CommentAddedEvent @event)
        {
            var comment = new CommentEntity
            {
                PostId = @event.PostId,
                Id = @event.CommentId,
                CreatedDate = @event.CreatedDate,
                Comment = @event.CommentText,
                Username = @event.Username,
                Edited = false
            };

            await _commentRepository.CreateAsync(comment);
        }

        public async Task On(CommentUpdatedEvent @event)
        {
            var comment = await _commentRepository.GetByIdAsync(@event.CommentId);
            if (comment == null)
                return;

            comment.Comment = @event.CommentText;
            comment.Edited = true;
            comment.UpdatedDate = @event.UpdatedDate;

            await _commentRepository.UpdateAsync(comment);
        }

        public async Task On(CommentRemovedEvent @event)
        {
            await _commentRepository.DeleteAsync(@event.CommentId);
        }

        public async Task On(PostRemovedEvent @event)
        {
            await _postRepository.DeleteAsync(@event.PostId);
        }
    }
}
