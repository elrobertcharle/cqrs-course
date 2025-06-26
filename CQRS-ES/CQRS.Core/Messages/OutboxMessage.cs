using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Messages
{
    public class OutboxMessage
    {
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string EventType { get; set; } = null!;

        public string AggregateType { get; set; } = null!;

        [BsonRepresentation(BsonType.String)]
        public Guid AggregateId { get; set; }

        public int Version { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Serialized JSON of the event
        /// </summary>
        public string Payload { get; set; } = null!;

        public bool IsPublished { get; set; } = false;

        public DateTime? PublishedAt { get; set; }
    }
}
