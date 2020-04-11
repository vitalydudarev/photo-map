using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Service.Storage;

namespace Storage.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly ILogger<StorageController> _logger;
        private readonly IStorage _storage;

        public StorageController(ILoggerFactory loggerFactory, IStorage storage)
        {
            _logger = loggerFactory.CreateLogger<StorageController>();
            _storage = storage;
        }

        [HttpGet]
        [Route("file/{key}")]
        public async Task<ByteArrayContent> GetFileAsync(string key)
        {
            _logger.LogInformation("Request received: " + key);
            
            var bytes = await _storage.GetAsync(key);

            return new ByteArrayContent(bytes);
        }

        [HttpPost]
        [Route("file")]
        public async Task SaveFileAsync(string key, byte[] bytes)
        {
            
        }
    }
}