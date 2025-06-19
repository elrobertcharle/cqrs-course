using CQRS.Core.Queries;
using MediatR;
using Post.Query.Api.Database.Entities;

namespace Post.Query.Api.Queries
{
    public class FindPostByIdQuery : BaseQuery, IRequest<List<PostEntity>>
    {
        public Guid Id { get; set; }
    }
}
