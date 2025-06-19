using CQRS.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LikePostController : ControllerBase
    {
        private readonly ILogger<LikePostController> _logger;
        private readonly IMediator _mediator;

        public LikePostController(ILogger<LikePostController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> LikePost(Guid postId, CancellationToken ct)
        {
            await _mediator.Send(new LikePostCommand { Id = postId }, ct);

            return Ok();
        }
    }
}
