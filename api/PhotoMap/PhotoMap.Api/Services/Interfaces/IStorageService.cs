using System.Threading.Tasks;

namespace PhotoMap.Api.Services.Interfaces
{
    public interface IStorageService
    {
        Task<byte[]> GetFileAsync(long fileId);

        Task<FileInfo> GetFileInfoAsync(long fileId);

        Task DeleteFileAsync(long fileId);

        Task DeleteAllFilesAsync();
    }
}
