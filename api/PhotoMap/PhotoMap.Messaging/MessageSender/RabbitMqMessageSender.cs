using System;
using System.Text;
using Microsoft.Extensions.Logging;
using PhotoMap.Messaging.Commands;
using RabbitMQ.Client;

namespace PhotoMap.Messaging.MessageSender
{
    public class RabbitMqMessageSender : IMessageSender, IDisposable
    {
        private readonly RabbitMqConfiguration _rabbitMqConfiguration;
        private readonly ILogger<RabbitMqMessageSender> _logger;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqMessageSender(RabbitMqConfiguration configuration, ILogger<RabbitMqMessageSender> logger)
        {
            _rabbitMqConfiguration = configuration;
            _logger = logger;
        }

        public void Send(CommandBase commandBase)
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

            _channel.QueueDeclare(queue: _rabbitMqConfiguration.ResponseQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var serializedCommand = commandBase.Serialize();
            var body = Encoding.UTF8.GetBytes(serializedCommand);

            _channel.BasicPublish(exchange: "",
                routingKey: _rabbitMqConfiguration.ResponseQueueName,
                basicProperties: null,
                body: body);

            _logger.LogInformation("Message sent.");
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();

            _logger.LogInformation("Connection closed.");
        }
    }
}
