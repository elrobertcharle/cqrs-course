namespace Post.Cmd.Api.Commands
{
    public interface ICommandHandler
    {
        public Task HandleAsync(NewPostCommand command);
        public Task HandleAsync(EditMessageCommand command);
        public Task HandleAsync(LikePostCommand command);
        public Task HandleAsync(AddCommentCommand command);
        public Task HandleAsync(EditCommentCommand command);
        public Task HandleAsync(RemoveCommandCommand command);
        public Task HandleAsync(DeletePostCommand command);
        public Task HandleAsync(RestoreReadDbCommand command);
    }
}
