using CQRS.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AddCommentController : ControllerBase
    {
        private readonly ILogger<EditMessageController> _logger;
        private readonly IMediator _mediator;

        public AddCommentController(ILogger<EditMessageController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> AddComment(Guid postId, AddCommentCommand command, CancellationToken ct) // roberto: arreglar nombre de parametros en url
        {
            command.Id = postId;
            await _mediator.Send(command, ct);
            return Ok();
        }
    }
}
