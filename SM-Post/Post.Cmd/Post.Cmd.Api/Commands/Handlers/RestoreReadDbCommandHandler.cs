using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers
{
    public class RestoreReadDbCommandHandler : IRequestHandler<RestoreReadDbCommand>
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public RestoreReadDbCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task Handle(RestoreReadDbCommand request, CancellationToken ct)
        {
            await _eventSourcingHandler.RepublishEventAsync(ct);
        }
    }
}
