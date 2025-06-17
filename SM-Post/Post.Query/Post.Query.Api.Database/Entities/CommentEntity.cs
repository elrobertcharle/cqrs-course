using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Api.Database.Entities
{
    [Table("comments")]
    public class CommentEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string Comment { get; set; } = null!;

        public bool Edited { get; set; }

        [ForeignKey(nameof(Post))]
        public Guid PostId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual PostEntity Post { get; set; } = null!;
    }
}
