using CQRS.Core.Commands;
using MediatR;

namespace Post.Cmd.Api.Commands
{
    public class DeletePostCommand : BaseCommand, IRequest
    {
        public string Username { get; set; } = null!;
    }
}
