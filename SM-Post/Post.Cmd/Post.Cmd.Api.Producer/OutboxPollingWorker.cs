namespace Post.Cmd.Api.Producer;

public class OutboxPollingWorker : BackgroundService
{
    private readonly ILogger<OutboxPollingWorker> _logger;

    public OutboxPollingWorker(ILogger<OutboxPollingWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    if (_logger.IsEnabled(LogLevel.Information))
        //    {
        //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //    }
        //    await Task.Delay(1000, stoppingToken);
        //}
    }
}
