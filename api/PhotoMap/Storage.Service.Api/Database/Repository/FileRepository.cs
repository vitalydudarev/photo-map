using System.Threading.Tasks;
using Storage.Service.Database.Entities;

namespace Storage.Service.Database.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly FileContext _context;

        public FileRepository(FileContext context)
        {
            _context = context;
        }
        
        public async Task<File> AddAsync(File incomingFile)
        {
            var entityEntry = await _context.Files.AddAsync(incomingFile);

            return entityEntry.Entity;
        }

        public async Task<File> GetAsync(long fileId)
        {
            return await _context.Files.FindAsync(fileId);
        }

        public async Task DeleteAsync(long fileId)
        {
            throw new System.NotImplementedException();
        }
    }
}