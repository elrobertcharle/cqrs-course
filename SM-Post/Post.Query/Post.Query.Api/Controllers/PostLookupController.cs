using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostLookupController : ControllerBase
    {
        private readonly ILogger<PostLookupController> _logger;
        private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

        public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPost()
        {
            var posts = await _queryDispatcher.SendAsync(new FindAllPostQuery());
            return Ok(posts);
        }

        [HttpGet("byId/{postId}")]
        public async Task<IActionResult> GetByPostId(Guid postId)
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery { Id = postId });
            return Ok(posts);
        }

        [HttpGet("byAuthor/{author}")]
        public async Task<IActionResult> GetByAuthor(string author)
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostByAuthorQuery { Author = author });
            return Ok(posts);
        }

        [HttpGet("withComments")]
        public async Task<IActionResult> GetPostsWithComments()
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());
            return Ok(posts);
        }

        [HttpGet("withLikes/{numberOfLikes}")]  //roberto: I dont like this
        public async Task<IActionResult> GetPostsWithLikes(int numberOfLikes)
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes });
            return Ok(posts);
        }
    }
}
