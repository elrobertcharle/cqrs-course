using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public interface IEventStoreRepository
    {
        Task SaveAsync(EventModel @event, CancellationToken ct);
        Task<List<EventModel>> FindByAggregateIdAsync(Guid aggregateId, CancellationToken ct);
        Task<List<EventModel>> FindAllAsync(CancellationToken ct);
    }
}
