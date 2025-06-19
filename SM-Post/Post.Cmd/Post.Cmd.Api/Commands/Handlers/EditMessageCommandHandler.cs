using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers
{
    public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand>
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public EditMessageCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task Handle(EditMessageCommand command, CancellationToken ct)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id, ct);
            aggregate.EditMessage(command.Message);
            await _eventSourcingHandler.SaveAsync(aggregate, ct);
        }
    }
}
