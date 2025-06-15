using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Domain.Entities
{
    [Table("Posts")]
    public class PostEntity
    {
        [Key]
        public Guid PostId { get; set; }
        public string Author { get; set; } = null!;
        public DateTime DatePosted { get; set; }
        public string Message { get; set; } = null!;
        public int Likes { get; set; }
        public virtual ICollection<CommentEntity> Comments { get; set; } = null!;
    }
}
