using System.Collections.Generic;
using System.Threading.Tasks;
using Storage.Service.Database.Entities;

namespace Storage.Service.Database.Repository
{
    public interface IFileRepository
    {
        Task<File> AddAsync(File incomingFile);
        Task<File> GetAsync(long fileId);
        Task<File> GetByFileNameAsync(string fileName);
        Task<IEnumerable<File>> GetAllAsync();
        Task DeleteAsync(long fileId);
        Task DeleteAllAsync();
    }
}
