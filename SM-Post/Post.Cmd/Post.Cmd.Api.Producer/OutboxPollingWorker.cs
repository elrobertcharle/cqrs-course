using Microsoft.Extensions.DependencyInjection;
using Post.Cmd.Api.Producer.Handlers;
using System.Runtime.Intrinsics.X86;

namespace Post.Cmd.Api.Producer;

public class OutboxPollingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxPollingWorker> _logger;

    public OutboxPollingWorker(ILogger<OutboxPollingWorker> logger, IServiceScopeFactory ssf)
    {
        _serviceScopeFactory = ssf;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var outboxMessageHandler = scope.ServiceProvider.GetService<IOutboxEventHandler>();
        if (outboxMessageHandler == null)
            throw new InvalidOperationException($"Service implementation was not found. Interface={nameof(IOutboxEventHandler)}.");

        while (!ct.IsCancellationRequested)
        {
            await outboxMessageHandler.SendPendingAsync(ct);
            await Task.Delay(10000, ct);
        }
    }
}
