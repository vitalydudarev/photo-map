using System.Threading.Tasks;

namespace Image.Service.Services.StorageService
{
    public interface IStorageService
    {
        Task<byte[]> GetFileAsync(long fileId);
    }
}
