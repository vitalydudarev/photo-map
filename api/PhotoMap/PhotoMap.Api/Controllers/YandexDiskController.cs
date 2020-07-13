using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/yandex-disk")]
    public class YandexDiskController : ControllerBase
    {
        private readonly IMessageSender _messageSender;
        private readonly ILogger<YandexDiskController> _logger;

        public YandexDiskController(IMessageSender messageSender, ILogger<YandexDiskController> logger)
        {
            _messageSender = messageSender;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult RunProcessing([FromBody] string accessToken)
        {
            var runProcessingCommand = new RunProcessingCommand { UserId = 1, Token = accessToken };

            _messageSender.Send(runProcessingCommand);

            return Ok();
        }
    }
}
