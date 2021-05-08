using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Messaging.MessageSender;
using Yandex.Disk.Api.Client;
using ConvertImageEvent = PhotoMap.Api.Commands.ConvertImageEvent;
using PauseProcessingEvent = PhotoMap.Api.Commands.PauseProcessingEvent;
using StartProcessingEvent = PhotoMap.Api.Commands.StartProcessingEvent;
using YandexDiskUserIdentifier = PhotoMap.Api.Models.YandexDiskUserIdentifier;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/yandex-disk")]
    public class YandexDiskController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IMessageSender _messageSender;
        private readonly IConvertedImageHolder _convertedImageHolder;

        public YandexDiskController(
            IUserService userService,
            IPhotoService photoService,
            IMessageSender messageSender,
            IConvertedImageHolder convertedImageHolder)
        {
            _userService = userService;
            _photoService = photoService;
            _messageSender = messageSender;
            _convertedImageHolder = convertedImageHolder;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> StartProcessing([FromBody] int userId)
        {
            var user = await _userService.GetAsync(userId);
            var startProcessingCommand = new StartProcessingEvent
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
            var pauseProcessingCommand = new PauseProcessingEvent
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
                var commandId = Guid.NewGuid();
                var convertImageCommand = new ConvertImageEvent
                {
                    Id = commandId,
                    FileContents = bytes
                };

                _messageSender.Send(convertImageCommand);

                const int maxTimeout = 5000;
                int waitTime = 0;
                byte[] convertedBytes;

                do
                {
                    await Task.Delay(1000);
                    convertedBytes = _convertedImageHolder.Get(commandId);
                    waitTime += 1000;
                } while (waitTime <= maxTimeout || convertedBytes == null);

                if (convertedBytes != null)
                    return new FileContentResult(convertedBytes, "image/jpg");

                return BadRequest();
            }

            return new FileContentResult(bytes, "image/jpg");
        }
    }
}
