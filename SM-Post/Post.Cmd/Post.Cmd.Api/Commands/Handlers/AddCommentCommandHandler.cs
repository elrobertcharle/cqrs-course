using CQRS.Core.Handlers;
using MediatR;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers
{
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand>
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public AddCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task Handle(AddCommentCommand command, CancellationToken ct)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id, ct);
            aggregate.AddComment(command.Comment, command.Username);
            await _eventSourcingHandler.SaveAsync(aggregate, ct);
        }
    }
}
