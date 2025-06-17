using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Api.Database.Entities
{
    [Table("posts")]
    public class PostEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Author { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string Message { get; set; } = null!;
        public int Likes { get; set; }
        public ICollection<CommentEntity> Comments { get; set; } = null!;
    }
}
