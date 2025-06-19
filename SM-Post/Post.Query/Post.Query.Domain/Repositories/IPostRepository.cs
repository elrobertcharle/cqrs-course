using Post.Query.Api.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Domain.Repositories
{
    public interface IPostRepository
    {
        Task CreateAsync(PostEntity post, CancellationToken ct);
        Task UpdateAsync(PostEntity post, CancellationToken ct);
        Task DeleteAsync(Guid postId, CancellationToken ct);
        Task<PostEntity?> GetByIdAsync(Guid postId, CancellationToken ct);
        Task<List<PostEntity>> ListAllAsync(CancellationToken ct);
        Task<List<PostEntity>> ListByAuthorAsync(string author, CancellationToken ct);
        Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes, CancellationToken ct);
        Task<List<PostEntity>> ListWithCommentsAsync(CancellationToken ct);
    }
}
