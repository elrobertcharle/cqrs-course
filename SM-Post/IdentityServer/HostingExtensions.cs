using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
        var postgresOptions = new PostgresDbOptions();
        builder.Configuration.GetSection("Db").Bind(postgresOptions);
        var connectionString = PostgresDbOptions.GetConnectionString(postgresOptions);
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(connectionString, b =>
                {
                    b.MigrationsAssembly(migrationsAssembly);
                });
            })
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = b => b.UseNpgsql(connectionString, b =>
            {
                b.MigrationsAssembly(migrationsAssembly);
            });
        })

            .AddAspNetIdentity<ApplicationUser>()
            .AddLicenseSummary();

        builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                // register your IdentityServer with Google at https://console.developers.google.com
                // enable the Google+ API
                // set the redirect URI to https://localhost:5001/signin-google
                options.ClientId = "copy client ID from Google here";
                options.ClientSecret = "copy client secret from Google here";
            });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        InitializeDatabase(app);

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }

    /// <summary>
    /// dotnet ef migrations add Initial -c ApplicationDbContext -o Data/Migrations/ApplicationDbContextMigrations
    /// dotnet ef migrations add Initial -c PersistedGrantDbContext -o Data/Migrations/PersistedGrantDbContextMigrations
    /// dotnet ef migrations add Initial -c ConfigurationDbContext -o Data/Migrations/ConfigurationDbContextMigrations
    /// 
    /// dotnet ef database update -c ApplicationDbContext
    /// dotnet ef database update -c PersistedGrantDbContext
    /// dotnet ef database update -c ConfigurationDbContext
    /// </summary>
    /// <param name="app"></param>
    private static void InitializeDatabase(IApplicationBuilder app)
    {
        DelteAll(app);

        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }

    private static void DelteAll(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
        {

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            context.ApiResources.RemoveRange(context.ApiResources);

            context.ApiScopes.RemoveRange(context.ApiScopes);

            context.IdentityResources.RemoveRange(context.IdentityResources);

            context.Clients.RemoveRange(context.Clients);

            context.SaveChanges();
        }
    }
}
