using CQRS.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DeletePostController : ControllerBase
    {
        private readonly ILogger<DeletePostController> _logger;
        private readonly IMediator _mediator;

        public DeletePostController(ILogger<DeletePostController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId, DeletePostCommand command, CancellationToken ct)
        {
            command.Id = postId;
            await _mediator.Send(command, ct);

            return Ok();
        }
    }
}
