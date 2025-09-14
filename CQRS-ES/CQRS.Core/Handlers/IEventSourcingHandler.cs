using CQRS.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Handlers
{
    public interface IEventSourcingHandler<TAggregate>
    {
        Task SaveAsync(AggregateRoot aggregate, CancellationToken ct);
        Task<TAggregate> GetByIdAsync(Guid aggregateId, CancellationToken ct);
        Task RepublishEventAsync(CancellationToken ct);
    }
}
