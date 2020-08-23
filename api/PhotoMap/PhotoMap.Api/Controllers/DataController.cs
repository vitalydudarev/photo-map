using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhotoMap.Api.Database.Services;
using PhotoMap.Api.ServiceClients.StorageService;

namespace PhotoMap.Api.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IStorageService _storageService;

        public DataController(IPhotoService photoService, IStorageService storageService)
        {
            _photoService = photoService;
            _storageService = storageService;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllDataAsync()
        {
            await _photoService.DeleteAllAsync();
            await _storageService.DeleteAllFilesAsync();

            return NoContent();
        }
    }
}
