using System.Threading.Tasks;
using Image.Service.Services.DTOs;

namespace Image.Service.Services.StorageService
{
    public interface IStorageService
    {
        Task<byte[]> GetFileAsync(long fileId);

        Task DeleteFileAsync(long fileId);

        Task<StorageServiceFileDto> SaveFileAsync(string fileName, byte[] fileContents);
    }
}
