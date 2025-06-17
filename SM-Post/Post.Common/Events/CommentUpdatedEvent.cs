using CQRS.Core.Events;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Common.Events
{
    public class CommentUpdatedEvent : BaseEvent
    {
        public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent))
        {
        }

        [BsonRepresentation(BsonType.String)]
        public Guid PostId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid CommentId { get; set; }

        public string CommentText { get; set; } = null!;
        
        public string Username { get; set; } = null!;
        
        public DateTime UpdatedDate { get; set; }
    }
}
