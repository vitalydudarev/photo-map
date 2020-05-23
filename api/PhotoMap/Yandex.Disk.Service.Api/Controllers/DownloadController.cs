using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Yandex.Disk.Service.Api.Controllers
{
    [ApiController]
    [Route("api/yandex-disk/[controller]")]
    public class DownloadController : ControllerBase
    {
        public DownloadController(ILoggerFactory loggerFactory)
        {
            
        }
    }
}