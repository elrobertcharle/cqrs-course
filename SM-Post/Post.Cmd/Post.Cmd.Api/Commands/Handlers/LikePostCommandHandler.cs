using CQRS.Core.Handlers;
using MediatR;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers
{
    public class LikePostCommandHandler : IRequestHandler<LikePostCommand>
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public LikePostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task Handle(LikePostCommand command, CancellationToken ct)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id, ct);
            aggregate.LikedPost();
            await _eventSourcingHandler.SaveAsync(aggregate, ct);
        }
    }
}
