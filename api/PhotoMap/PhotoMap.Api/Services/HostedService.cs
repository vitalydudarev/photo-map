using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoMap.Messaging.MessageListener;

namespace PhotoMap.Api.Services
{
    public class HostedService : BackgroundService
    {
        private readonly ILogger<HostedService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HostedService(ILogger<HostedService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
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

            using var scope = _serviceScopeFactory.CreateScope();

            var messageListener = scope.ServiceProvider.GetService<IMessageListener>();
            messageListener.Listen(stoppingToken);

            await Task.CompletedTask;
        }
    }
}
