using System.Threading.Tasks;
using Storage.Service.Database.Entities;

namespace Storage.Service.Database.Repository
{
    public interface IFileRepository
    {
        Task<File> AddAsync(File incomingFile);
        Task<File> GetAsync(long fileId);
        Task DeleteAsync(long fileId);
    }
}