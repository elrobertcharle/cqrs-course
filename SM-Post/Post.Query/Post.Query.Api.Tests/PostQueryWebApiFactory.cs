using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MongoDB.Driver.Core.Configuration;
using Post.Query.Api.Database;
using Post.Query.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Api.Tests
{
    public class PostQueryWebApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _postgresConnectionString;

        public PostQueryWebApiFactory(string postgresConnectionString)
        {
            _postgresConnectionString = postgresConnectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                var dbContextDescriptor = services.First(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));
                services.Remove(dbContextDescriptor);
                services.AddDbContext<DatabaseContext>(ob => ob.UseNpgsql(_postgresConnectionString));

                var databaseContextFactory = services.First(d => d.ServiceType == typeof(DatabaseContextFactory));
                services.Remove(databaseContextFactory);
                services.AddSingleton(new DatabaseContextFactory(ob => ob.UseNpgsql(_postgresConnectionString)));
            });
        }
    }
}
