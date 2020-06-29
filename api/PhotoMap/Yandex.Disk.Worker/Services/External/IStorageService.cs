using System.Threading.Tasks;

namespace Yandex.Disk.Worker.Services.External
{
    public interface IStorageService
    {
        Task SaveFileAsync(string fileName, byte[] fileContents);
    }
}
