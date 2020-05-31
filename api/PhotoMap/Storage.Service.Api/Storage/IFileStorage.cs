using System.Threading.Tasks;

namespace Storage.Service.Storage
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(string fileName, byte[] bytes);
        Task<byte[]> GetAsync(string fileName);
        void Delete(string fileName);
    }
}