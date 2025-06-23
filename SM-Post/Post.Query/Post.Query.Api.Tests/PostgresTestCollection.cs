using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Api.Tests
{
    [CollectionDefinition("PostgresTestCollection")]
    public class PostgresTestCollection : ICollectionFixture<PostgresDatabaseFixture>
    {
    }
}
