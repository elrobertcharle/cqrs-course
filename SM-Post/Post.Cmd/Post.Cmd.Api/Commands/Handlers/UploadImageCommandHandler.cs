using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers
{
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand>
    {
        private readonly IEventSourcingHandler<UploadImageAggregate> _eventSourcingHandler;

        public UploadImageCommandHandler(IEventSourcingHandler<UploadImageAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task Handle(UploadImageCommand command, CancellationToken ct)
        {
            var aggregate = new UploadImageAggregate(command.Id, command.Title);
            await _eventSourcingHandler.SaveAsync(aggregate, ct);
        }
    }
}
