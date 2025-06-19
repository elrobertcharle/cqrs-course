using CQRS.Core.Handlers;
using MediatR;
using MongoDB.Driver;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers
{
    public class NewPostCommandHandler : IRequestHandler<NewPostCommand>
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public NewPostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task Handle(NewPostCommand command, CancellationToken ct)
        {
            var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
            await _eventSourcingHandler.SaveAsync(aggregate, ct);
        }
    }
}
