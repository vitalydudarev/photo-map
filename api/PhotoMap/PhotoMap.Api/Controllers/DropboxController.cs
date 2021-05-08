using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dropbox.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Messaging.MessageSender;
using ConvertImageEvent = PhotoMap.Api.Commands.ConvertImageEvent;
using DropboxUserIdentifier = PhotoMap.Api.Models.DropboxUserIdentifier;
using PauseProcessingEvent = PhotoMap.Api.Commands.PauseProcessingEvent;
using StartProcessingEvent = PhotoMap.Api.Commands.StartProcessingEvent;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/dropbox")]
    public class DropboxController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IMessageSender _messageSender;
        private readonly IConvertedImageHolder _convertedImageHolder;

        public DropboxController(
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
                UserIdentifier = new DropboxUserIdentifier { UserId = user.Id },
                Token = user.DropboxAccessToken
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
                UserIdentifier = new DropboxUserIdentifier { UserId = user.Id }
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

            var config = new DropboxClientConfig("PhotoMap") { HttpClient = httpClient };
            var dropboxClient = new DropboxClient(user.DropboxAccessToken, config);

            var fileMetadata = await dropboxClient.Files.DownloadAsync(photo.Path);
            var fileContents = await fileMetadata.GetContentAsByteArrayAsync();

            if (photo.FileName.ToUpper().EndsWith("HEIC"))
            {
                var commandId = Guid.NewGuid();
                var convertImageCommand = new ConvertImageEvent
                {
                    Id = commandId,
                    FileContents = fileContents
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

            return new FileContentResult(fileContents, "image/jpg");
        }
    }
}
