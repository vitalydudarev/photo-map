using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoMap.Api.Database.Entities;

namespace PhotoMap.Api.Database.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly PhotoMapContext _context;

        public PhotoService(PhotoMapContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Photo photo)
        {
            await _context.Photos.AddAsync(photo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Photo>> GetByUserIdAsync(int userId)
        {
            return await _context.Photos.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task DeleteByUserId(int userId)
        {
            var entities = await _context.Photos.Where(a => a.UserId == userId).ToListAsync();
            _context.Photos.RemoveRange(entities);
        }
    }
}
