using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoMap.Api.Database.Entities;
using PhotoMap.Api.DTOs;

namespace PhotoMap.Api.Database.Services
{
    public interface IPhotoService
    {
        Task AddAsync(Photo photo);

        Task<Photo> GetAsync(int id);

        Task<Photo> GetByFileNameAsync(string fileName);

        Task<PagedResponse<PhotoDto>> GetByUserIdAsync(int userId, int top, int skip);

        Task DeleteByUserId(int userId);

        Task DeleteAllAsync();
    }
}
