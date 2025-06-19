using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Consumer.Handlers
{
    public interface IEventHandler<TEvent>
    {
        Task HandleAsync(TEvent @event, CancellationToken ct);
    }
}
