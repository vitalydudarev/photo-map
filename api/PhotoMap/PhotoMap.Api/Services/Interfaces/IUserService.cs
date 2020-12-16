using System.Threading.Tasks;
using PhotoMap.Api.DTOs;

namespace PhotoMap.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task AddAsync(AddUserDto addUserDto);

        Task<UserDto> GetAsync(int id);

        Task UpdateAsync(int id, UpdateUserDto updateUserDto);
    }
}
