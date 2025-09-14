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
    public class ImageUploadedEvent : BaseEvent
    {
        public ImageUploadedEvent() : base(nameof(ImageUploadedEvent))
        {
        }

        [BsonRepresentation(BsonType.String)]
        public Guid ImageId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
