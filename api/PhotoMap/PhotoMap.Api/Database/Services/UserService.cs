using System;
using System.Threading.Tasks;
using PhotoMap.Api.Database.Entities;
using PhotoMap.Api.DTOs;

namespace PhotoMap.Api.Database.Services
{
    public class UserService : IUserService
    {
        private readonly PhotoMapContext _context;

        public UserService(PhotoMapContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AddUserDto addUserDto)
        {
            var user = await _context.Users.FindAsync(addUserDto.Id);
            if (user == null)
            {
                user = new User
                {
                    Name = addUserDto.Name,
                    YandexDiskToken = addUserDto.YandexDiskAccessToken,
                    YandexDiskTokenExpiresOn = DateTimeOffset.UtcNow.AddSeconds(addUserDto.YandexDiskTokenExpiresIn)
                };
                await _context.Users.AddAsync(user);
            }
            else
            {
                user.YandexDiskToken = addUserDto.YandexDiskAccessToken;
                user.YandexDiskTokenExpiresOn = DateTimeOffset.UtcNow.AddSeconds(addUserDto.YandexDiskTokenExpiresIn);

                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<UserDto> GetAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                YandexDiskAccessToken = user.YandexDiskToken,
                YandexDiskTokenExpiresOn = user.YandexDiskTokenExpiresOn,
                YandexDiskStatus = user.YandexDiskStatus
            };
        }

        public async Task UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.YandexDiskStatus = updateUserDto.Status;

                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();
        }
    }
}
