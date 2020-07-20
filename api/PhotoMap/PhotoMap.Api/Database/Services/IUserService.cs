using System.Threading.Tasks;
using PhotoMap.Api.DTOs;

namespace PhotoMap.Api.Database.Services
{
    public interface IUserService
    {
        Task AddAsync(AddUserDto addUserDto);

        Task<UserDto> GetAsync(int id);
    }
}
