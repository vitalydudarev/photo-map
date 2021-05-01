using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhotoMap.Messaging.EventHandlerManager;
using PhotoMap.Messaging.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace PhotoMap.Messaging.MessageListener
{
    public class RabbitMqMessageListener : IMessageListener, IDisposable
    {
        private readonly RabbitMqConfiguration _rabbitMqConfiguration;
        private readonly IEventHandlerManager _eventHandlerManager;
        private readonly ILogger<RabbitMqMessageListener> _logger;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqMessageListener(
            RabbitMqConfiguration rabbitMqConfiguration,
            IEventHandlerManager eventHandlerManager,
            ILogger<RabbitMqMessageListener> logger)
        {
            _rabbitMqConfiguration = rabbitMqConfiguration;
            _eventHandlerManager = eventHandlerManager;
            _logger = logger;

            InitializeConnection();
        }

        public void Listen(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (bc, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                _logger.LogInformation($"Processing message: '{message}'.");

                try
                {
                    var command = EventBase.Deserialize(message);
                    var commandHandler = _eventHandlerManager.GetHandler(command);
                    if (commandHandler != null)
                    {
                        Task.Run(async () => await commandHandler.HandleAsync(command, cancellationToken),
                            cancellationToken);
                    }

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

            _channel.BasicConsume(queue: _rabbitMqConfiguration.ConsumeQueueName, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();

            _logger.LogInformation("Connection closed.");
        }

        private void InitializeConnection()
        {
            var connectionFactory = new ConnectionFactory
            {
                UserName = _rabbitMqConfiguration.UserName,
                Password = _rabbitMqConfiguration.Password,
                HostName = _rabbitMqConfiguration.HostName,
                Port = _rabbitMqConfiguration.Port
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_rabbitMqConfiguration.ConsumeQueueName, durable: false, exclusive: false, autoDelete: false);
            _channel.BasicQos(0, 1, false);

            _logger.LogInformation($"Queue [{_rabbitMqConfiguration.ConsumeQueueName}] is waiting for messages.");
        }
    }
}
