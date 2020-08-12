using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoMap.Api.Database.Services;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using Yandex.Disk.Api.Client;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/yandex-disk")]
    public class YandexDiskController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IMessageSender _messageSender;
        private readonly ILogger<YandexDiskController> _logger;

        public YandexDiskController(
            IUserService userService,
            IPhotoService photoService,
            IMessageSender messageSender,
            ILogger<YandexDiskController> logger)
        {
            _userService = userService;
            _photoService = photoService;
            _messageSender = messageSender;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RunProcessing(int userId)
        {
            var user = await _userService.GetAsync(userId);
            var runProcessingCommand = new RunProcessingCommand { UserId = userId, Token = user.YandexDiskAccessToken };

            _messageSender.Send(runProcessingCommand);

            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> StopProcessing(int userId)
        {
            var user = await _userService.GetAsync(userId);
            var stopProcessingCommand = new StopProcessingCommand { UserId = userId };

            _messageSender.Send(stopProcessingCommand);

            return NoContent();
        }

        [HttpGet("photos/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPhotoAsync(int id)
        {
            var photo = await _photoService.GetAsync(id);
            var user = await _userService.GetAsync(photo.UserId);

            var httpClient = new HttpClient();
            var yandexDiskApiClient = new ApiClient(user.YandexDiskAccessToken, httpClient);
            var downloadUrl = await yandexDiskApiClient.GetDownloadUrlAsync(photo.Path, new CancellationToken());

            var bytes = await httpClient.GetByteArrayAsync(downloadUrl.Href);

            return new FileContentResult(bytes, "image/jpg");
        }
    }
}
