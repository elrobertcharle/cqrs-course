using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LikePostController : ControllerBase
    {
        private readonly ILogger<LikePostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public LikePostController(ILogger<LikePostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> LikePost(Guid postId)
        {
            await _commandDispatcher.SendAsync(new LikePostCommand { Id = postId });

            return Ok();
        }
    }
}
