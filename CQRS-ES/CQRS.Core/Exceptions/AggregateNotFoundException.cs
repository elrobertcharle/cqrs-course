using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Exceptions
{
    public class AggregateNotFoundException : Exception
    {
        public Guid AggregateId { get; set; }

        public AggregateNotFoundException(string message) : base(message)
        {

        }

        public AggregateNotFoundException(Guid aggregateId) : base($"The aggregate with Id={aggregateId} was not found.")
        {
            AggregateId = aggregateId;
        }
    }
}
