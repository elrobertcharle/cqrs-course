using CQRS.Core.Handlers;
using MediatR;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers
{
    public class RemoveCommentCommandHandler : IRequestHandler<RemoveCommentCommand>
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public RemoveCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task Handle(RemoveCommentCommand command, CancellationToken ct)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id, ct);
            aggregate.RemoveComment(command.CommentId, command.Username);
            await _eventSourcingHandler.SaveAsync(aggregate, ct);
        }
    }
}
