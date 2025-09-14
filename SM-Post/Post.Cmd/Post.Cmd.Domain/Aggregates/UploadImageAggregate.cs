using CQRS.Core.Domain;
using Post.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Domain.Aggregates
{
    [EventStoreCollection("UploadImageEvents")]
    public class UploadImageAggregate : AggregateRoot
    {
        public UploadImageAggregate()
        {
        }

        public UploadImageAggregate(Guid imageId, string title)
        {
            RaiseEvent(new ImageUploadedEvent
            {
                ImageId = imageId,
                Title = title,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
