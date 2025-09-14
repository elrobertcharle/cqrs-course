using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Post.Cmd.Api;
using Post.Cmd.Api.Options;
using Post.Cmd.Api.Validators;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;
using Post.Common.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<PostMessageUpdatedEvent>();
BsonClassMap.RegisterClassMap<PostLikedEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentRemovedEvent>();
BsonClassMap.RegisterClassMap<PostRemovedEvent>();


builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));

builder.Services.AddScoped<EventStoreRepository<PostAggregate>>();
builder.Services.AddScoped<EventStoreRepository<UploadImageAggregate>>();

builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<PostAggregateEventStore>();
builder.Services.AddScoped<UploadImageAggregateEventStore>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>, PostEventSourcingHandler>();
builder.Services.AddScoped<IEventSourcingHandler<UploadImageAggregate>, UploadImageEventSourcingHandler>();

builder.Services.AddSingleton<IValidator<KafkaProducerOptions>, KafkaProducerOptionsValidator>();
builder.Services.AddOptions<KafkaProducerOptions>().Bind(builder.Configuration.GetSection("Kafka"));


builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<MongoDbConfig>>().Value;
    return new MongoClient(config.ConnectionString);
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = "https://localhost:5001";
    options.TokenValidationParameters.ValidateAudience = true;
    options.TokenValidationParameters.ValidAudience = "wgb_command_api";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("write", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "post_query_api.write");
    });
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUIApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Only if needed
    });
});

var app = builder.Build();

app.UseCors("AllowUIApp");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();