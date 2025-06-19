using MediatR;
using Post.Query.Api.Database.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries.Handlers
{
    public class FindPostByAuthorQueryHandler : IRequestHandler<FindPostByAuthorQuery, List<PostEntity>>
    {
        private readonly IPostRepository _postRepository;

        public FindPostByAuthorQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostEntity>> Handle(FindPostByAuthorQuery query, CancellationToken ct)
        {
            return await _postRepository.ListByAuthorAsync(query.Author, ct);
        }
    }
}
