using System.IO;
using System.Threading.Tasks;
using PhotoMap.Worker.Services.Definitions;
using PhotoMap.Worker.Services.DTOs;

namespace PhotoMap.Worker.Services.Implementations
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IStorageService _storageService;

        public ImageUploadService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<StorageServiceFileDto> SaveImageAsync(byte[] bytes, string fileName, string userName, string source)
        {
            var filePath = Path.Combine(source, userName, fileName);

            return await _storageService.SaveFileAsync(filePath, bytes);
        }

        public async Task<StorageServiceFileDto> SaveThumbnailAsync(byte[] bytes, string fileName, string userName, string source, int size)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var thumbFileName = $"{fileNameWithoutExtension}_{size}{extension}";
            var path = Path.Combine(source, userName, "thumbs", thumbFileName);

            return await _storageService.SaveFileAsync(path, bytes);
        }
    }
}
