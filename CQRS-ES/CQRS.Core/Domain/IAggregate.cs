using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public interface IAggregate
    {
        static abstract string GetCollectionName();
    }
}
