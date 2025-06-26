using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Producers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Post.Cmd.Api.Options;
using Post.Cmd.Infrastructure.Repositories;
using Post.Common.Events.JsonConverters;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;

namespace Post.Cmd.Api
{
    public class OutboxWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OutboxWorker> _logger;

        public OutboxWorker(IServiceScopeFactory ssf, ILogger<OutboxWorker> logger)
        {
            _serviceScopeFactory = ssf;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var outboxMessageHandler = scope.ServiceProvider.GetService<IOutboxMessageHandler>();
            if (outboxMessageHandler == null)
                throw new InvalidOperationException($"Service implementation was not found. Interface={nameof(IOutboxMessageHandler)}.");

            while (!ct.IsCancellationRequested)
            {
                await outboxMessageHandler.SendPendingAsync(ct);
                await Task.Delay(5000, ct);
            }
        }
    }
}
