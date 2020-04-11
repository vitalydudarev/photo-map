using System.Threading.Tasks;

namespace Storage.Service.Storage
{
    public interface IStorage
    {
        Task SaveAsync(string fileKey, byte[] bytes);
        Task<byte[]> GetAsync(string fileKey);
    }
}