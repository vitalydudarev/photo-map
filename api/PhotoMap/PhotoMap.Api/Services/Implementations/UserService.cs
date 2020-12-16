using System;
using System.Threading.Tasks;
using PhotoMap.Api.Database;
using PhotoMap.Api.Database.Entities;
using PhotoMap.Api.DTOs;
using PhotoMap.Api.Services.Interfaces;

namespace PhotoMap.Api.Services.Implementations
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
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
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
                YandexDiskTokenExpiresOn = user.YandexDiskTokenExpiresOn?.UtcDateTime,
                YandexDiskStatus = user.YandexDiskStatus,
                DropboxAccessToken = user.DropboxToken,
                DropboxTokenExpiresOn = user.DropboxTokenExpiresOn?.UtcDateTime,
                DropboxStatus = user.DropboxStatus
            };
        }

        public async Task UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(updateUserDto.YandexDiskToken))
                    user.YandexDiskToken = updateUserDto.YandexDiskToken;
                if (updateUserDto.YandexDiskTokenExpiresIn.HasValue)
                    user.YandexDiskTokenExpiresOn =
                        DateTimeOffset.UtcNow.AddSeconds(updateUserDto.YandexDiskTokenExpiresIn.Value);
                if (updateUserDto.YandexDiskStatus.HasValue)
                    user.YandexDiskStatus = updateUserDto.YandexDiskStatus.Value;

                if (!string.IsNullOrEmpty(updateUserDto.DropboxToken))
                {
                    user.DropboxToken = updateUserDto.DropboxToken;

                    user.DropboxTokenExpiresOn = updateUserDto.DropboxTokenExpiresIn.HasValue
                        ? DateTimeOffset.UtcNow.AddSeconds(updateUserDto.DropboxTokenExpiresIn.Value)
                        : DateTimeOffset.MaxValue;
                }

                if (updateUserDto.DropboxStatus.HasValue)
                    user.DropboxStatus = updateUserDto.DropboxStatus.Value;

                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();
        }
    }
}
