using MediatR;
using Post.Query.Api.Database.Entities;
using Post.Query.Domain.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Post.Query.Api.Queries.Handlers
{
    public class FindPostsWithLikesQueryHandler : IRequestHandler<FindPostsWithLikesQuery, List<PostEntity>>
    {
        private readonly IPostRepository _postRepository;

        public FindPostsWithLikesQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostEntity>> Handle(FindPostsWithLikesQuery query, CancellationToken ct)
        {
            return await _postRepository.ListWithLikesAsync(query.NumberOfLikes, ct);
        }
    }
}
