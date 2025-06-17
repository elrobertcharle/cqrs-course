using CQRS.Core.Messages;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Events
{
    public abstract class BaseEvent
    {
        protected BaseEvent(string type)
        {
            Type = type;
        }

        [BsonRepresentation(BsonType.String)]
        public Guid EventId { get; set; } = new Guid();

        public int Version { get; set; }
        
        public string Type { get; set; } = null!;
    }
}
