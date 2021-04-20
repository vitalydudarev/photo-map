using System.Threading.Tasks;
using PhotoMap.Worker.Services.DTOs;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IStorageService
    {
        Task<byte[]> GetFileAsync(long fileId);

        Task<StorageServiceFileDto> SaveFileAsync(string fileName, byte[] fileContents);

        Task DeleteFileAsync(long fileId);
    }
}
