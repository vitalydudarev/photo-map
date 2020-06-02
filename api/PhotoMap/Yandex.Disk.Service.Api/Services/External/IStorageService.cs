using System.Threading.Tasks;

namespace Yandex.Disk.Service.Api.Services.External
{
    public interface IStorageService
    {
        Task UploadFileAsync(string fileName, byte[] fileContents);
    }
}
