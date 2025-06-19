using MediatR;
using Post.Query.Api.Database.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries.Handlers
{
    public class FindPostByIdQueryHandler : IRequestHandler<FindPostByIdQuery, List<PostEntity>>
    {
        private readonly IPostRepository _postRepository;

        public FindPostByIdQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostEntity>> Handle(FindPostByIdQuery query, CancellationToken ct)
        {
            var post = await _postRepository.GetByIdAsync(query.Id, ct);
            var result = new List<PostEntity>();
            if (post != null)
                result.Add(post);
            return result;
        }
    }
}
