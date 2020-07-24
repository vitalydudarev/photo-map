using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoMap.Api.Database.Entities;

namespace PhotoMap.Api.Database.Services
{
    public interface IPhotoService
    {
        Task AddAsync(Photo photo);

        Task<IEnumerable<Photo>> GetByUserIdAsync(int userId);
    }
}
