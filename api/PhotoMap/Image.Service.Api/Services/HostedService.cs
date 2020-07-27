using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoMap.Messaging.MessageListener;

namespace Image.Service.Services
{
    public class HostedService : BackgroundService
    {
        private readonly ILogger<HostedService> _logger;
        private readonly IMessageListener _messageListener;

        public HostedService(IMessageListener messageListener, ILogger<HostedService> logger)
        {
            _messageListener = messageListener;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service running.");

            return base.StartAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _logger.LogInformation("ExecuteAsync");

            _messageListener.Listen(stoppingToken);

            await Task.CompletedTask;
        }

        // TODO:
        // 1. Add to RabbitMQ settings Durable, Autodelete, Exclusive
        // 2. try-catch for RabbitMQ connection init
        // 3. Remove InitializeConnection
    }
}
