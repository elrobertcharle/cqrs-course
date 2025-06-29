using System;


namespace CQRS.Core.Events
{
    public class NewOutboxCreatedEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TargetEventId { get; set; }
    }
}
