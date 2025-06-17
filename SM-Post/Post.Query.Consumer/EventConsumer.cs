using CQRS.Core.Consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Consumer
{
    public class EventConsumer : IEventConsumer
    {
        public void Consume(string topic)
        {
            throw new NotImplementedException();
        }
    }
}
