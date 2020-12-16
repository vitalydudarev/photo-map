using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhotoMap.Api.Services.Interfaces;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public PhotosController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id)
        {
            var bytes = await _storageService.GetFileAsync(id);

            return new FileContentResult(bytes, "image/jpg");
        }
    }
}
