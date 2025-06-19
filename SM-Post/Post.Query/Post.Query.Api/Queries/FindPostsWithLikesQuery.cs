using CQRS.Core.Queries;
using MediatR;
using Post.Query.Api.Database.Entities;

namespace Post.Query.Api.Queries
{
    public class FindPostsWithLikesQuery : BaseQuery, IRequest<List<PostEntity>>
    {
        public int NumberOfLikes { get; set; }
    }
}
