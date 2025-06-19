using CQRS.Core.Handlers;
using MediatR;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public DeletePostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task Handle(DeletePostCommand command, CancellationToken ct)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id, ct);
            aggregate.DeletePost(command.Username);
            await _eventSourcingHandler.SaveAsync(aggregate, ct);
        }
    }
}
