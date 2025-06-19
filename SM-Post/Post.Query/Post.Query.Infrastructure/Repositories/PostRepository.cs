using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Database;
using Post.Query.Api.Database.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContextFactory _contextFactory;

        public PostRepository(DatabaseContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateAsync(PostEntity post, CancellationToken ct)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            context.Posts.Add(post);
            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid postId, CancellationToken ct)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            var post = await GetByIdAsync(postId, ct);

            if (post == null)
                return;
            context.Posts.Remove(post);
            await context.SaveChangesAsync(ct);
        }

        public async Task<PostEntity?> GetByIdAsync(Guid postId, CancellationToken ct)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postId, ct);
        }

        public async Task<List<PostEntity>> ListAllAsync(CancellationToken ct)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).AsNoTracking().ToListAsync(ct);
        }

        public async Task<List<PostEntity>> ListByAuthorAsync(string author, CancellationToken ct)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).AsNoTracking().Where(p => p.Author.Contains(author)).ToListAsync(ct);
        }

        public async Task<List<PostEntity>> ListWithCommentsAsync(CancellationToken ct)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).AsNoTracking().Where(p => p.Comments != null && p.Comments.Any()).ToListAsync(ct);
        }

        public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes, CancellationToken ct)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).AsNoTracking().Where(p => p.Likes >= numberOfLikes).ToListAsync(ct);
        }

        public async Task UpdateAsync(PostEntity post, CancellationToken ct)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            context.Posts.Update(post);
            await context.SaveChangesAsync(ct);
        }
    }
}
