using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using PhotoMap.Messaging.Commands;
using RabbitMQ.Client;

namespace PhotoMap.Messaging.MessageSender
{
    public class RabbitMqMessageSender2 : IMessageSender2, IDisposable
    {
        private readonly Dictionary<string, RabbitMqConfiguration> _configurations;
        private readonly ILogger<RabbitMqMessageSender2> _logger;
        private readonly Dictionary<string, IConnection> _connections = new Dictionary<string, IConnection>();
        private readonly Dictionary<string ,IModel> _channels = new Dictionary<string, IModel>();

        public RabbitMqMessageSender2(
            Dictionary<string, RabbitMqConfiguration> configurations,
            ILogger<RabbitMqMessageSender2> logger)
        {
            _configurations = configurations;
            _logger = logger;
        }

        public void Send(CommandBase commandBase, string consumerApi)
        {
            if (_configurations.TryGetValue(consumerApi, out var configuration))
            {
                if (!_connections.TryGetValue(consumerApi, out _))
                    TryCreateConnection(configuration, consumerApi);

                try
                {
                    var serializedCommand = commandBase.Serialize();
                    var body = Encoding.UTF8.GetBytes(serializedCommand);

                    var channel = _channels[consumerApi];
                    channel.BasicPublish(exchange: "",
                        routingKey: configuration.ResponseQueueName,
                        basicProperties: null,
                        body: body);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to send message: {e.Message}");
                    return;
                }

                _logger.LogInformation($"Message to {consumerApi} sent.");
            }
            else
                _logger.LogWarning($"RabbitMQ configuration for {consumerApi} not found.");
        }

        public void Dispose()
        {
            var keys = _connections.Keys.ToList();

            foreach (var key in keys)
            {
                var configuration = _configurations[key];

                _channels[key].Close();
                _connections[key].Close();

                _logger.LogInformation($"RabbitMQ connection to {configuration.HostName}:{configuration.Port}, queue \'{configuration.ResponseQueueName}\' closed.");
            }
        }

        private void TryCreateConnection(RabbitMqConfiguration configuration, string consumerApi)
        {
            const int retryCount = 3;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    CreateConnection(configuration, consumerApi);

                    return;
                }
                catch (Exception e)
                {
                    if (i == retryCount - 1)
                    {
                        var errorMessage =
                            $"Unable to establish RabbitMQ connection to {configuration.HostName}:{configuration.Port}, queue {configuration.ResponseQueueName}: {e.Message}";
                        _logger.LogError(errorMessage);

                        throw new Exception(errorMessage);
                    }
                }
            }
        }

        private void CreateConnection(RabbitMqConfiguration configuration, string consumerApi)
        {
            var connectionFactory = new ConnectionFactory
            {
                UserName = configuration.UserName,
                Password = configuration.Password,
                HostName = configuration.HostName,
                Port = configuration.Port
            };

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: configuration.ResponseQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _connections.Add(consumerApi, connection);
            _channels.Add(consumerApi, channel);
        }
    }
}
