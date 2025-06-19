using CQRS.Core.Commands;
using MediatR;

namespace Post.Cmd.Api.Commands
{
    public class EditCommentCommand : BaseCommand, IRequest
    {
        public Guid CommentId { get; set; }
        public string Comment { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}
