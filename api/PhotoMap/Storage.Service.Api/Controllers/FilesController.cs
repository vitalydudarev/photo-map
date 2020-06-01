using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Service.Service;

namespace Storage.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService, ILogger<FilesController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpGet]
        [Route("{fileId}")]
        public async Task<IActionResult> GetFileAsync(long fileId)
        {
            _logger.LogInformation("GetFileAsync: Request received: fileId - " + fileId);

            var bytes = await _fileService.GetFileContentsAsync(fileId);
            if (bytes != null)
                return File(bytes, "image/jpeg");

            return NotFound(fileId);
        }

        [HttpGet]
        [Route("{fileId}/info")]
        public async Task<IActionResult> GetFileInfoAsync(long fileId)
        {
            _logger.LogInformation("GetFileInfoAsync: request received: fileId - " + fileId);

            var fileInfo = await _fileService.GetFileInfoAsync(fileId);
            if (fileInfo != null)
                return Ok(fileInfo);

            return NotFound(fileId);
        }

        [HttpPost]
        public async Task<IActionResult> SaveFileAsync([FromForm] IFormFile file)
        {
            _logger.LogInformation("SaveFileAsync: Request received: fileName - " + file.FileName);

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();
                var result = await _fileService.SaveAsync(file.FileName, bytes);

                return Created("uri", result);
            }
        }
    }
}
