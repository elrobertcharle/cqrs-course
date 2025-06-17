using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Database.Entities;

namespace Post.Query.Api.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<PostEntity> Posts { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }
    }
}
