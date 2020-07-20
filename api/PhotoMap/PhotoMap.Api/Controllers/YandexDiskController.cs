using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoMap.Api.Database.Services;
using PhotoMap.Api.DTOs;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/yandex-disk")]
    public class YandexDiskController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMessageSender _messageSender;
        private readonly ILogger<YandexDiskController> _logger;

        public YandexDiskController(
            IUserService userService,
            // IMessageSender messageSender,
            ILogger<YandexDiskController> logger)
        {
            _userService = userService;
            // _messageSender = messageSender;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddTokenAsync([FromBody] AddUserDto addUserDto)
        {
            await _userService.AddAsync(addUserDto);

            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var user = await _userService.GetAsync(id);
            if (user != null)
                return Ok(user);

            return NotFound();
        }

        /*[HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult RunProcessing([FromBody] string accessToken)
        {
            var runProcessingCommand = new RunProcessingCommand { UserId = 1, Token = accessToken };

            _messageSender.Send(runProcessingCommand);

            return Ok();
        }*/
    }
}
