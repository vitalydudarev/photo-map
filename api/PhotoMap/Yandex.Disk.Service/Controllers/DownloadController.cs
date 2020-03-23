using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Yandex.Disk.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloadController : ControllerBase
    {
        public DownloadController(ILoggerFactory loggerFactory)
        {
            
        }
    }
}