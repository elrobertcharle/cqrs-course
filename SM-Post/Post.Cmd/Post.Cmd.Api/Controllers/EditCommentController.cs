using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EditCommentController : ControllerBase
    {
        private readonly ILogger<EditCommentController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public EditCommentController(ILogger<EditCommentController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> EditComment(Guid postId, EditCommentCommand command) // roberto: arreglar nombre de parametros en url
        {
            command.Id = postId;
            await _commandDispatcher.SendAsync(command);
            return Ok();
        }
    }
}
