using System;
using System.Collections.Generic;
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
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqMessageSender2(
            Dictionary<string, RabbitMqConfiguration> configurations,
            ILogger<RabbitMqMessageSender2> logger)
        {
            _configurations = configurations;
            _logger = logger;
        }

        public void Send(CommandBase commandBase, string consumerApi)
        {
            var configuration = _configurations[consumerApi];

            var connectionFactory = new ConnectionFactory
            {
                UserName = configuration.UserName,
                Password = configuration.Password,
                HostName = configuration.HostName,
                Port = configuration.Port
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: configuration.ResponseQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var serializedCommand = commandBase.Serialize();
            var body = Encoding.UTF8.GetBytes(serializedCommand);

            _channel.BasicPublish(exchange: "",
                routingKey: configuration.ResponseQueueName,
                basicProperties: null,
                body: body);

            _logger.LogInformation("Message sent.");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();

            _logger.LogInformation("Connection closed.");
        }
    }
}
