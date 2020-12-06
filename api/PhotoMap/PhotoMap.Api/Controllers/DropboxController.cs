using System.Net.Http;
using System.Threading.Tasks;
using Dropbox.Api;
using GraphicsLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoMap.Api.Database.Services;
using PhotoMap.Common.Commands;
using PhotoMap.Common.Models;
using PhotoMap.Messaging.MessageSender;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/dropbox")]
    public class DropboxController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IMessageSender _messageSender;
        private readonly ILogger<DropboxController> _logger;

        public DropboxController(
            IUserService userService,
            IPhotoService photoService,
            IMessageSender messageSender,
            ILogger<DropboxController> logger)
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
            var pauseProcessingCommand = new PauseProcessingCommand
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
                var imageProcessor = new ImageProcessor(fileContents);
                return new FileContentResult(imageProcessor.GetImageBytes(), "image/jpg");
            }

            return new FileContentResult(fileContents, "image/jpg");
        }
    }
}
