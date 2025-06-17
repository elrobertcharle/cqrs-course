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
    public class PostMessageUpdatedEvent : BaseEvent
    {
        public PostMessageUpdatedEvent() : base(nameof(PostMessageUpdatedEvent))
        {
        }

        [BsonRepresentation(BsonType.String)]
        public Guid PostId { get; set; }
        public string Message { get; set; } = null!;
    }
}
