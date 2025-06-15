using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string? _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

        public bool Active
        {
            get => _active; set => _active = value;
        }

        public PostAggregate()
        {
        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _author = @event.Author;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void EditMessage(string message)
        {
            if (!_active)
                throw new InvalidOperationException("You cannot edit a message of an inactive post.");

            if (string.IsNullOrWhiteSpace(message))
                throw new InvalidOperationException();

            RaiseEvent(new MessageUpdatedEvent
            {
                Id = _id,
                Message = message
            });
        }

        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void LikedPost()
        {
            if (!_active)
                throw new InvalidOperationException();

            RaiseEvent(new PostLikedEvent
            {
                Id = _id
            });
        }

        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="username"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddComment(string comment, string username)
        {
            if (!_active)
                throw new InvalidOperationException();

            if (string.IsNullOrWhiteSpace(comment))
                throw new InvalidOperationException();

            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                CommentId = Guid.NewGuid(),
                CommentText = comment,
                Username = username,
                CommentDate = DateTime.Now
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.CommentText, @event.Username));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="comment"></param>
        /// <param name="username"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!_active)
                throw new InvalidOperationException();

            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
                throw new InvalidOperationException($"The user {username} cannot modify the comment. CommentId={commentId}.");

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                CommentText = comment,
                Username = username,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event.CommentText, @event.Username);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="username"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void RemoveComment(Guid commentId, string username)
        {
            if (!_active)
                throw new InvalidOperationException();

            if (!_comments.TryGetValue(commentId, out Tuple<string, string>? value))
                return;

            if (!value.Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
                throw new InvalidOperationException($"The user {username} cannot remove the comment. CommentId={commentId}.");

            RaiseEvent(new CommentRemovedEvent
            {
                Id = _id,
                CommentId = commentId
            });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void DeletePost(string username)
        {
            if (!_active)
                throw new InvalidOperationException();

            if (_author == null)
                throw new InvalidOperationException();

            if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
                throw new InvalidOperationException($"The user {username} cannot delete the post. PostId={Id}.");

            RaiseEvent(new PostRemovedEvent
            {
                Id = _id
            });
        }

        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}
