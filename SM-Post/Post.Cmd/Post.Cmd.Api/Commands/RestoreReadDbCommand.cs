using CQRS.Core.Commands;
using MediatR;

namespace Post.Cmd.Api.Commands
{
    public class RestoreReadDbCommand : BaseCommand, IRequest
    {
    }
}
