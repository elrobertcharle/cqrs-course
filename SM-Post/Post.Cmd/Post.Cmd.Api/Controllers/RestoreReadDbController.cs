using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RestoreReadDbController : ControllerBase
    {
        private readonly ILogger<RestoreReadDbController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public RestoreReadDbController(ILogger<RestoreReadDbController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> RestoreReadDb()
        {
            await _commandDispatcher.SendAsync(new RestoreReadDbCommand());

            return Ok();
        }
    }
}
