using System.Threading.Tasks;
using Yandex.Disk.Worker.Services.DTOs;

namespace Yandex.Disk.Worker.Services.External
{
    public interface IStorageService
    {
        Task<StorageServiceFileDto> SaveFileAsync(string fileName, byte[] fileContents);
    }
}
