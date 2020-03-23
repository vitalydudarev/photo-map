using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Storage.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly ILogger<StorageController> _logger;

        public StorageController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<StorageController>();
        }
    }
}