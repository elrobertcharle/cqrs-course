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
    public class CommentAddedEvent : BaseEvent
    {
        public CommentAddedEvent() : base(nameof(CommentAddedEvent))
        {
        }

        [BsonRepresentation(BsonType.String)]
        public Guid CommentId { get; set; }
        public string CommentText { get; set; } = null!;
        public string Username { get; set; } = null!;
        public DateTime CommentDate { get; set; }
    }
}
