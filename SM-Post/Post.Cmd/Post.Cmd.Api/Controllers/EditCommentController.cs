using CQRS.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EditCommentController : ControllerBase
    {
        private readonly ILogger<EditCommentController> _logger;
        private readonly IMediator _mediator;

        public EditCommentController(ILogger<EditCommentController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> EditComment(Guid postId, EditCommentCommand command, CancellationToken ct) // roberto: arreglar nombre de parametros en url
        {
            command.Id = postId;
            await _mediator.Send(command, ct);
            return Ok();
        }
    }
}
