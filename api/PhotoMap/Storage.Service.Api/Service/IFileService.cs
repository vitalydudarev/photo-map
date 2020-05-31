using System.Threading.Tasks;
using Storage.Service.Models;

namespace Storage.Service.Service
{
    public interface IFileService
    {
        Task<OutgoingFile> SaveAsync(string fileName, byte[] fileContents);
        Task<byte[]> GetAsync(long fileId);
    }
}