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
    public class PostCreatedEvent : BaseEvent
    {
        public PostCreatedEvent() : base(nameof(PostCreatedEvent))
        {
        }

        [BsonRepresentation(BsonType.String)]
        public Guid PostId { get; set; }

        public string Author { get; set; } = null!;

        public string Message { get; set; } = null!;

        public DateTime DatePosted { get; set; }
    }
}
