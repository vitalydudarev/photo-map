using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Yandex.Disk.Worker.Services
{
    public class HostedService : BackgroundService
    {
        private readonly ILogger<HostedService> _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly RabbitMqSettings _rabbitMqSettings;

        public HostedService(IOptions<RabbitMqSettings> rabbitMqOptions, ILogger<HostedService> logger)
        {
            _rabbitMqSettings = rabbitMqOptions.Value;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service running.");

            var connectionFactory = new ConnectionFactory
            {
                UserName = _rabbitMqSettings.UserName,
                Password = _rabbitMqSettings.Password,
                HostName = _rabbitMqSettings.HostName,
                Port = _rabbitMqSettings.Port,
                DispatchConsumersAsync = true
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_rabbitMqSettings.IncomingQueueName);
            _channel.BasicQos(0, 1, false);

            _logger.LogInformation($"Queue [{_rabbitMqSettings.IncomingQueueName}] is waiting for messages.");

            return base.StartAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);

            _channel.Close();
            _connection.Close();

            _logger.LogInformation("RabbitMQ connection is closed.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (bc, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                _logger.LogInformation($"Processing message: '{message}'.");

                try
                {
                    var runProcessingCommand = JsonSerializer.Deserialize<RunProcessingCommand>(message);
                    _logger.LogInformation($"Token: {runProcessingCommand.Token}");

                    await Task.Delay(new Random().Next(1, 3) * 1000, stoppingToken);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (JsonException)
                {
                    _logger.LogError($"JSON Parse Error: '{message}'.");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
                catch (AlreadyClosedException)
                {
                    _logger.LogInformation("RabbitMQ is closed!");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An error has occurred: {e.Message}");
                }
            };

            _channel.BasicConsume(queue: _rabbitMqSettings.IncomingQueueName, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }
    }
}
