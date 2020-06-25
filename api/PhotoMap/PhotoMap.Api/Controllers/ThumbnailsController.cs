using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoMap.Api.Services;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/thumbs")]
    public class ThumbnailsController : ControllerBase
    {
        private readonly IThumbnailService _thumbnailService;

        public ThumbnailsController(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        [HttpGet("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(string key)
        {
            var bytes = await _thumbnailService.GetContentsAsync(key);

            return new FileContentResult(bytes, "image/jpg");
        }
    }
}
