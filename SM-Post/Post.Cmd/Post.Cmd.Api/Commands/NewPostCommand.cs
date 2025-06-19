using CQRS.Core.Commands;
using MediatR;

namespace Post.Cmd.Api.Commands
{
    public class NewPostCommand : BaseCommand, IRequest
    {
        public string Author { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
