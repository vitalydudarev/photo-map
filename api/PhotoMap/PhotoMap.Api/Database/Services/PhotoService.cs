using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoMap.Api.Database.Entities;
using PhotoMap.Api.DTOs;

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

        public async Task<Photo> GetAsync(int id)
        {
            return await _context.Photos.FindAsync(id);
        }

        public async Task<PagedResponse<PhotoDto>> GetByUserIdAsync(int userId, int top, int skip)
        {
            var photos = await _context.Photos
                .Where(a => a.UserId == userId)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            var totalRecords = await _context.Photos.CountAsync(a => a.UserId == userId);

            var values = photos.Select(a => new PhotoDto
            {
                DateTimeTaken = a.DateTimeTaken,
                FileName = a.FileName,
                Id = a.Id,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                PhotoUrl = "yandex-disk/photos/" + a.Id,
                ThumbnailLargeFileId = a.ThumbnailLargeFileId,
                ThumbnailSmallFileId = a.ThumbnailSmallFileId,
                ThumbnailUrl = "photos/" + a.ThumbnailSmallFileId
            }).ToArray();

            return new PagedResponse<PhotoDto> { Values = values, Limit = top, Offset = skip, Total = totalRecords };
        }

        public async Task DeleteByUserId(int userId)
        {
            var entities = await _context.Photos.Where(a => a.UserId == userId).ToListAsync();
            _context.Photos.RemoveRange(entities);
        }

        public async Task DeleteAllAsync()
        {
            var entities = await _context.Photos.ToListAsync();
            _context.Photos.RemoveRange(entities);

            await _context.SaveChangesAsync();
        }
    }
}
