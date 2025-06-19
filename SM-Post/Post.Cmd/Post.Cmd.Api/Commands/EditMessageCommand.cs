using CQRS.Core.Commands;
using MediatR;

namespace Post.Cmd.Api.Commands
{
    public class EditMessageCommand : BaseCommand, IRequest
    {
        public string Message { get; set; } = null!;

    }
}
