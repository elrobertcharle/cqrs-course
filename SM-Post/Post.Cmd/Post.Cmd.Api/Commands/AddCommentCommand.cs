using CQRS.Core.Commands;
using MediatR;

namespace Post.Cmd.Api.Commands
{
    public class AddCommentCommand : BaseCommand, IRequest
    {
        public string Comment { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}
