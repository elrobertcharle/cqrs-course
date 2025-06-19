using MediatR;
using Post.Query.Api.Database.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries.Handlers
{
    public class FindPostsWithCommentsQueryHandler : IRequestHandler<FindPostsWithCommentsQuery, List<PostEntity>>
    {
        private readonly IPostRepository _postRepository;

        public FindPostsWithCommentsQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostEntity>> Handle(FindPostsWithCommentsQuery query, CancellationToken ct)
        {
            return await _postRepository.ListWithCommentsAsync(ct);
        }
    }
}
