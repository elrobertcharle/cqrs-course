using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Post.Query.Api.Database;
using Post.Query.Api.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Post.Query.Api.Tests
{
    public class PostgresDatabaseFixture : IDisposable
    {
        private readonly string _adminConnectionString;
        public string TestConnectionString { get; }

        private readonly string _testDbName = $"test_{Guid.NewGuid():N}";

        public PostQueryWebApiFactory AppFactory { get; }

        public PostgresDatabaseFixture()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            var dbOptions = new PostgresDbOptions();
            configuration.GetSection("Db").Bind(dbOptions);
            _adminConnectionString = PostgresDbOptions.GetConnectionString(dbOptions);
            var adminConnection = new NpgsqlConnection(_adminConnectionString);
            adminConnection.Open();
            var command = adminConnection.CreateCommand();
            command.CommandText = $"CREATE DATABASE \"{_testDbName}\" TEMPLATE template1;";
            command.ExecuteNonQuery();
            adminConnection.Close();

            dbOptions.Database = _testDbName;
            TestConnectionString = PostgresDbOptions.GetConnectionString(dbOptions);

            // Apply migrations
            using var dbContext = new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().UseNpgsql(TestConnectionString).Options);
            dbContext.Database.Migrate();

            AppFactory = new PostQueryWebApiFactory(TestConnectionString);
        }

        public void Dispose()
        {
            using var adminConn = new NpgsqlConnection(_adminConnectionString);
            adminConn.Open();
            try
            {
                using (var terminateCmd = adminConn.CreateCommand())
                {
                    terminateCmd.CommandText = $@"
                SELECT pg_terminate_backend(pid) 
                FROM pg_stat_activity 
                WHERE datname = '{_testDbName}';";
                    terminateCmd.ExecuteNonQuery();
                }

                using (var dropCmd = adminConn.CreateCommand())
                {
                    dropCmd.CommandText = $@"DROP DATABASE IF EXISTS ""{_testDbName}"";";
                    dropCmd.ExecuteNonQuery();
                }
            }
            finally
            {
                adminConn.Close();
            }
        }
    }
}
