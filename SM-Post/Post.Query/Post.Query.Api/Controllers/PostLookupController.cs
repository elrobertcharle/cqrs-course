using CQRS.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Post.Query.Api.Database.Entities;
using Post.Query.Api.Queries;

namespace Post.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/lookup-posts")]
    public class PostLookupController : ControllerBase
    {
        private readonly ILogger<PostLookupController> _logger;
        private readonly IMediator _mediator;

        public PostLookupController(ILogger<PostLookupController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPost(CancellationToken ct)
        {
            var posts = await _mediator.Send(new FindAllPostQuery(), ct);
            return Ok(posts);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetByPostId(Guid postId, CancellationToken ct)
        {
            var posts = await _mediator.Send(new FindPostByIdQuery { Id = postId }, ct);
            return Ok(posts);
        }

        [HttpGet("by-author")]
        public async Task<IActionResult> GetByAuthor([FromQuery] string author, CancellationToken ct)
        {
            var posts = await _mediator.Send(new FindPostByAuthorQuery { Author = author }, ct);
            return Ok(posts);
        }

        [HttpGet("with-comments")]
        public async Task<IActionResult> GetPostsWithComments(CancellationToken ct)
        {
            var posts = await _mediator.Send(new FindPostsWithCommentsQuery(), ct);
            return Ok(posts);
        }

        [HttpGet("with-likes")]
        public async Task<IActionResult> GetPostsWithLikes([FromQuery(Name = "likes")] int numberOfLikes, CancellationToken ct)
        {
            var posts = await _mediator.Send(new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes }, ct);
            return Ok(posts);
        }
    }
}
