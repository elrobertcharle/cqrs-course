using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DeletePostController : ControllerBase
    {
        private readonly ILogger<DeletePostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public DeletePostController(ILogger<DeletePostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId, DeletePostCommand command)
        {
            command.Id = postId;
            await _commandDispatcher.SendAsync(command);

            return Ok();
        }
    }
}
