using CQRS.Core.Commands;
using MediatR;

namespace Post.Cmd.Api.Commands
{
    public class RemoveCommentCommand : BaseCommand, IRequest
    {
        public Guid CommentId { get; set; }
        public string Username { get; set; } = null!;
    }
}
