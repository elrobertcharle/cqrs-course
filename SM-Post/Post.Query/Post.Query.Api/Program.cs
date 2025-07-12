using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Database;
using Post.Query.Api.Database.Entities;
using Post.Query.Api.Middleware;
using Post.Query.Api.Options;
using Post.Query.Api.Queries;
//using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var postgresOptions = new PostgresDbOptions();
builder.Configuration.GetSection("Db").Bind(postgresOptions);
var connectionString = PostgresDbOptions.GetConnectionString(postgresOptions);
Action<DbContextOptionsBuilder> configureDbContextPsql = ob => ob.UseNpgsql(connectionString);

builder.Services.AddDbContext<DatabaseContext>(configureDbContextPsql);
builder.Services.AddSingleton(new DatabaseContextFactory(configureDbContextPsql));

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.Authority = "https://localhost:5001";  // roberto: put in config
    options.TokenValidationParameters.ValidateAudience = false;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "post_query_api.read");
    });
});


builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }