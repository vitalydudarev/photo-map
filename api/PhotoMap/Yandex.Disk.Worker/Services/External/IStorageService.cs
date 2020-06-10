using System.Threading.Tasks;

namespace Yandex.Disk.Worker.Services.External
{
    public interface IStorageService
    {
        Task UploadFileAsync(string fileName, byte[] fileContents);
    }
}
