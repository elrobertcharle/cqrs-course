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
    public class CommentRepository : ICommentRepository
    {
        private readonly DatabaseContextFactory _contextFactory;

        public CommentRepository(DatabaseContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateAsync(CommentEntity comment)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            context.Comments.Add(comment);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid commentId)
        {
            using var context = _contextFactory.CreateContext();
            var comment = await GetByIdAsync(commentId);
            if (comment == null)
                return;

            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }

        public async Task<CommentEntity?> GetByIdAsync(Guid commentId)
        {
            using var context = _contextFactory.CreateContext();
            return await context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task UpdateAsync(CommentEntity comment)
        {
            using var context = _contextFactory.CreateContext();
            context.Comments.Update(comment);
            await context.SaveChangesAsync();
        }
    }
}
