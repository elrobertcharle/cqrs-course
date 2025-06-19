using CQRS.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NewPostController : ControllerBase
    {
        private readonly ILogger<NewPostController> _logger;
        private readonly IMediator _mediator;

        public NewPostController(ILogger<NewPostController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> NewPost(NewPostCommand command, CancellationToken ct)
        {
            var postId = Guid.NewGuid();
            command.Id = postId;

            await _mediator.Send(command, ct);

            return StatusCode(StatusCodes.Status201Created, new NewPostResponse
            {
                Id = postId,
                Message = "Created"
            });
        }
    }
}
