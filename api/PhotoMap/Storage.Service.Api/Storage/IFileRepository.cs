using System.Threading.Tasks;

namespace Storage.Service.Storage
{
    public interface IFileRepository
    {
        Task AddAsync();
        Task GetAsync(int fileId);
        Task DeleteAsync(int fileId);
    }
}