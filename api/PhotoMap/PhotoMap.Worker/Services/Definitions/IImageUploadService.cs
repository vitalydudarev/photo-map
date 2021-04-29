using System.Threading.Tasks;
using PhotoMap.Worker.Services.DTOs;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IImageUploadService
    {
        Task<StorageServiceFileDto> SaveImageAsync(byte[] bytes, string fileName, string userName, string source);

        Task<StorageServiceFileDto> SaveThumbnailAsync(byte[] bytes, string fileName, string userName, string source,
            int size);
    }
}
