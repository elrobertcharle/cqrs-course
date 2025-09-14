using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EventStoreCollectionAttribute : Attribute
    {
        public string CollectionName { get; }

        public EventStoreCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
