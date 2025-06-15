using CQRS.Core.Infrastructure;
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
        private readonly ICommandDispatcher _commandDispatcher;

        public AddCommentController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> AddComment(Guid postId, AddCommentCommand command) // roberto: arreglar nombre de parametros en url
        {
            command.Id = postId;
            await _commandDispatcher.SendAsync(command);
            return Ok();
        }
    }
}
