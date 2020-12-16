using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphicsLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Common.Commands;
using PhotoMap.Common.Models;
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
        public async Task<IActionResult> StartProcessing([FromBody] int userId)
        {
            var user = await _userService.GetAsync(userId);
            var startProcessingCommand = new StartProcessingCommand
            {
                UserIdentifier = new YandexDiskUserIdentifier { UserId = user.Id },
                Token = user.YandexDiskAccessToken
            };

            _messageSender.Send(startProcessingCommand);

            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PauseProcessing(int userId)
        {
            var user = await _userService.GetAsync(userId);
            var pauseProcessingCommand = new PauseProcessingCommand
            {
                UserIdentifier = new YandexDiskUserIdentifier { UserId = user.Id }
            };

            _messageSender.Send(pauseProcessingCommand);

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

            if (photo.FileName.ToUpper().EndsWith("HEIC"))
            {
                var imageProcessor = new ImageProcessor(bytes);
                return new FileContentResult(imageProcessor.GetImageBytes(), "image/jpg");
            }

            return new FileContentResult(bytes, "image/jpg");
        }
    }
}
