using CQRS.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EditMessageController : ControllerBase
    {
        private readonly ILogger<EditMessageController> _logger;
        private readonly IMediator _mediator;

        public EditMessageController(ILogger<EditMessageController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessage(Guid id, EditMessageCommand command, CancellationToken ct)
        {
            command.Id = id;
            await _mediator.Send(command, ct);

            return Ok();
        }
    }
}
