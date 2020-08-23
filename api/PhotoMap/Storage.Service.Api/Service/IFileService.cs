using System.Threading.Tasks;
using Storage.Service.Models;

namespace Storage.Service.Service
{
    public interface IFileService
    {
        Task<OutgoingFile> SaveAsync(string fileName, byte[] fileContents);
        Task<byte[]> GetFileContentsAsync(long fileId);
        Task<OutgoingFileInfo> GetFileInfoAsync(long fileId);
        Task DeleteFileAsync(long fileId);
        Task DeleteAllFilesAsync();
    }
}
