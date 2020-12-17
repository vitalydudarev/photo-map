using System.Threading.Tasks;

namespace PhotoMap.Api.Services.Interfaces
{
    public interface IFileProvider
    {
        Task<byte[]> GetFileContents(long fileId);
    }
}
