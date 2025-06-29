using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Api.Producer;
using Post.Cmd.Api.Producer.Handlers;
using Post.Cmd.Api.Producer.Options;
using Post.Cmd.Api.Producer.Validators;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IOutboxEventHandler, OutboxEventHandler>();
builder.Services.AddOptions<KafkaOptions>().Bind(builder.Configuration.GetSection("KafkaConfig"));
builder.Services.AddSingleton<IValidator<KafkaOptions>, KafkaOptionsValidator>();
builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection("MongoDbConfig"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
    return new MongoClient(config.ConnectionString);
});

//builder.Services.AddHostedService<OutboxPollingWorker>();
builder.Services.AddHostedService<KafkaOutboxListener>();

var host = builder.Build();
host.Run();
