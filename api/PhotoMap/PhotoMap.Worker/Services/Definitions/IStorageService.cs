using System.Threading.Tasks;
using PhotoMap.Worker.Services.DTOs;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IStorageService
    {
        Task<StorageServiceFileDto> SaveFileAsync(string fileName, byte[] fileContents);
    }
}
