using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhotoMap.Api.Services.Interfaces;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IFileProvider _fileProvider;

        public PhotosController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetAsync(long fileId)
        {
            var fileContents = await _fileProvider.GetFileContents(fileId);

            return new FileContentResult(fileContents, "image/jpg");
        }
    }
}
