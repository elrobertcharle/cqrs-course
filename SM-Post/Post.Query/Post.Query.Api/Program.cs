using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Database;
using Post.Query.Api.Database.Entities;
using Post.Query.Api.Middleware;
using Post.Query.Api.Queries;
//using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

Action<DbContextOptionsBuilder> configureDbContext = o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
Action<DbContextOptionsBuilder> configureDbContextPsql = o => o.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSql"));

//builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddDbContext<Post.Query.Api.Database.DatabaseContext>(configureDbContextPsql);
//builder.Services.AddSingleton(new DatabaseContextFactory(configureDbContext));
builder.Services.AddSingleton(new DatabaseContextFactory(configureDbContextPsql));

var databaseContext = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();