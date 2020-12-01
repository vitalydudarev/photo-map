using System.Threading.Tasks;

namespace PhotoMap.Api.ServiceClients.StorageService
{
    public interface IStorageService
    {
        Task<byte[]> GetFileAsync(long fileId);

        Task DeleteFileAsync(long fileId);

        Task DeleteAllFilesAsync();
    }
}
