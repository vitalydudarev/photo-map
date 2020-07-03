using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/yandex-disk")]
    public class YandexDiskController : ControllerBase
    {
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly ILogger<YandexDiskController> _logger;

        public YandexDiskController(IOptions<RabbitMqSettings> rabbitMqOptions, ILogger<YandexDiskController> logger)
        {
            _rabbitMqSettings = rabbitMqOptions.Value;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult RunProcessing([FromBody] string accessToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqSettings.HostName,
                UserName = _rabbitMqSettings.UserName,
                Password = _rabbitMqSettings.Password,
                Port = _rabbitMqSettings.Port
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _rabbitMqSettings.CommandsQueueName, durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            _logger.LogInformation($"Queue {_rabbitMqSettings.CommandsQueueName} initialized.");

            var runProcessingCommand = new RunProcessingCommand { Token = accessToken };
            var serialized = JsonConvert.SerializeObject(runProcessingCommand);
            var body = Encoding.UTF8.GetBytes(serialized);

            channel.BasicPublish(exchange: "", routingKey: _rabbitMqSettings.CommandsQueueName, basicProperties: null,
                body: body);

            _logger.LogInformation($"Message {runProcessingCommand} sent to {_rabbitMqSettings.CommandsQueueName} queue.");

            return Ok();
        }
    }
}
