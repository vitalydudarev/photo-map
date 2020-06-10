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
        private int executionCount = 0;
        private readonly ILogger<HostedService> _logger;
        private Timer _timer;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private const string QueueName = "ordering.emailworker";

        public HostedService(IOptions<RabbitMQSettings> rabbitMqOptions, ILogger<HostedService> logger)
        {
            _rabbitMQSettings = rabbitMqOptions.Value;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service running.");

            _connectionFactory = new ConnectionFactory
            {
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password,
                HostName = _rabbitMQSettings.HostName,
                DispatchConsumersAsync = true
            };

            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclarePassive(QueueName);
            _channel.BasicQos(0, 1, false);

            _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");

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

                _logger.LogInformation($"Processing msg: '{message}'.");

                try
                {
                    var order = JsonSerializer.Deserialize<Message>(message);
                    _logger.LogInformation($"Message ID: {order.Id}, text: {order.Text}");
                    // _logger.LogInformation($"Sending order #{order.Id} confirmation email to [{order.Email}].");

                    await Task.Delay(new Random().Next(1, 3) * 1000, stoppingToken); // simulate an async email process

                    // _logger.LogInformation($"Order #{order.Id} confirmation email sent.");
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
                    _logger.LogError(default, e, e.Message);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }
    }

    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}
