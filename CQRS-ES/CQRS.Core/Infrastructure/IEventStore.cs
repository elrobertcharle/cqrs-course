using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Infrastructure
{
    public interface IEventStore
    {
        Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion, CancellationToken ct);
        Task<List<BaseEvent>> GetEventAsync(Guid aggregateId, CancellationToken ct);
        Task<List<Guid>> GetAggregateIdsAsync(CancellationToken ct);
    }
}
