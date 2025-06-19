using CQRS.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RemoveCommentController : ControllerBase
    {
        private readonly ILogger<RemoveCommentController> _logger;
        private readonly IMediator _mediator;

        public RemoveCommentController(ILogger<RemoveCommentController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> RemoveComment(Guid postId, RemoveCommentCommand command, CancellationToken ct)
        {
            command.Id = postId;
            await _mediator.Send(command, ct);

            return Ok();
        }
    }
}
