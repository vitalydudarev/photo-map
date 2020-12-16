using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoMap.Api.Database;
using PhotoMap.Api.Database.Entities;
using PhotoMap.Api.DTOs;
using PhotoMap.Api.Services.Interfaces;

namespace PhotoMap.Api.Services.Implementations
{
    public class PhotoService : IPhotoService
    {
        private readonly PhotoMapContext _context;
        private readonly HostInfo _hostInfo;

        public PhotoService(PhotoMapContext context, HostInfo hostInfo)
        {
            _context = context;
            _hostInfo = hostInfo;
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

        public async Task<Photo> GetByFileNameAsync(string fileName)
        {
            return await _context.Photos.FirstOrDefaultAsync(a => a.FileName == fileName);
        }

        public async Task<PagedResponse<PhotoDto>> GetByUserIdAsync(int userId, int top, int skip)
        {
            var photos = await _context.Photos
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.DateTimeTaken)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            var totalRecords = await _context.Photos.CountAsync(a => a.UserId == userId);

            var url = GetApiUrl();

            static string Source(string s) =>
                s switch
                {
                    "Yandex.Disk" => "yandex-disk",
                    "Dropbox" => "dropbox",
                    _ => s
                };

            var values = photos.Select(a => new PhotoDto
            {
                DateTimeTaken = a.DateTimeTaken.UtcDateTime,
                FileName = a.FileName,
                Id = a.Id,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                PhotoUrl = $"{url}/{Source(a.Source)}/photos/" + a.Id,
                ThumbnailLargeUrl = $"{url}/photos/" + a.ThumbnailLargeFileId,
                ThumbnailSmallUrl = $"{url}/photos/" + a.ThumbnailSmallFileId
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

        private string GetApiUrl()
        {
            return _hostInfo.GetUrl() + "api";
        }
    }
}
