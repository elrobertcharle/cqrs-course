using CQRS.Core.Queries;
using MediatR;
using Post.Query.Api.Database.Entities;

namespace Post.Query.Api.Queries
{
    public class FindAllPostQuery : BaseQuery, IRequest<List<PostEntity>>
    {
    }
}
