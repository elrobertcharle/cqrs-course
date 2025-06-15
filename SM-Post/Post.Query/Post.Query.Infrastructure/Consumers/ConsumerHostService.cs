using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Consumers
{
    public class ConsumerHostService : IHostedService
    {
        private readonly ILogger<ConsumerHostService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ConsumerHostService(ILogger<ConsumerHostService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event consumer service running.");

            using IServiceScope scope = _serviceProvider.CreateScope();
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            if (topic == null)
                throw new InvalidOperationException("KAFKA_TOPIC was not set");

            Task.Run(() => eventConsumer.Consume(topic), cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event consumer service stopped.");
            return Task.CompletedTask;
        }
    }
}
