using CQRS.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Repositories
{
    public interface IPostEventStoreRepository : IEventStoreRepository
    {
    }
}
