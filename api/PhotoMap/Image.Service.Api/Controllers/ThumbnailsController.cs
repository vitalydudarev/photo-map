using System.Threading.Tasks;
using GraphicsLibrary;
using Microsoft.AspNetCore.Mvc;

namespace Image.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThumbnailsController : ControllerBase
    {
        [HttpPost]
        public async Task CreateThumbnailAsync(byte[] bytes, int size)
        {
            using (var imageProcessor = new ImageProcessor(bytes))
            {
                /*var thumbnailBytes = imageProcessor.Resize(size);
                var rotatedBytes = imageProcessor.Resize(size);*/
            }
        }
    }
}