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
    public class CommentRemovedEvent : BaseEvent
    {
        public CommentRemovedEvent() : base(nameof(CommentRemovedEvent))
        {
        }

        [BsonRepresentation(BsonType.String)]
        public Guid CommentId { get; set; }
    }
}
