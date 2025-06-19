using MediatR;
using Post.Query.Api.Database.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries.Handlers
{
    public class FindAllPostQueryHandler : IRequestHandler<FindAllPostQuery, List<PostEntity>>
    {
        private readonly IPostRepository _postRepository;

        public FindAllPostQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostEntity>> Handle(FindAllPostQuery query, CancellationToken ct)
        {
            return await _postRepository.ListAllAsync(ct);
        }
    }
}
