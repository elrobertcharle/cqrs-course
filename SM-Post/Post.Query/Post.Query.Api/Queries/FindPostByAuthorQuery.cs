using CQRS.Core.Queries;
using MediatR;
using Post.Query.Api.Database.Entities;

namespace Post.Query.Api.Queries
{
    public class FindPostByAuthorQuery : BaseQuery, IRequest<List<PostEntity>>
    {
        public string Author { get; set; } = null!;
    }
}
