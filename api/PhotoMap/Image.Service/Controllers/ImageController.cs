using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Image.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;

        public ImageController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ImageController>();
        }
    }
}