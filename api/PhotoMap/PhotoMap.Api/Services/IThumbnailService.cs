using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoMap.Api.Services
{
    public interface IThumbnailService
    {
        IEnumerable<string> GetFiles();
        int GetFilesCount();
        Task<byte[]> GetContentsAsync(string fileName);
    }
}
