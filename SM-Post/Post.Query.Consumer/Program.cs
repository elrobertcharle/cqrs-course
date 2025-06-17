using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Post.Common.Events;
using Post.Query.Api.Database;
using Post.Query.Consumer;
using Post.Query.Consumer.Config;
using Post.Query.Consumer.Handlers;
using Post.Query.Consumer.Validators;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
Action<DbContextOptionsBuilder> configureDbContextPsql = o => o.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSql"));
builder.Services.AddDbContext<DatabaseContext>(configureDbContextPsql);
builder.Services.AddSingleton<IValidator<KafkaConfig>, KafkaConfigValidator>();
builder.Services.AddScoped<IEventHandler<PostCreatedEvent>, PostCreatedEventHandler>();
builder.Services.AddOptions<KafkaConfig>().Bind(builder.Configuration.GetSection("KafkaConfig"));

var host = builder.Build();
host.Run();
