using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoMap.Api.ServiceClients.StorageService;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IStorageService _storageService;

        public ImagesController(ILoggerFactory loggerFactory, IStorageService storageService)
        {
            _logger = loggerFactory.CreateLogger<ImagesController>();
            _storageService = storageService;
        }
        
        [HttpGet]
        [Route("{key}")]
        public async Task<FileContentResult> GetFileAsync(string key)
        {
            var bytes = await _storageService.GetFileAsync(key);
            
            return new FileContentResult(bytes, "image/jpg");
        }
    }
}