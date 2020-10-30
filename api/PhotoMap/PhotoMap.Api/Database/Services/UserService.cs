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
                YandexDiskTokenExpiresOn = user.YandexDiskTokenExpiresOn,
                YandexDiskStatus = user.YandexDiskStatus,
                DropboxAccessToken = user.DropboxToken,
                DropboxTokenExpiresOn = user.DropboxTokenExpiresOn,
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
                if (updateUserDto.YandexDiskTokenExpiresOn.HasValue)
                    user.YandexDiskTokenExpiresOn = updateUserDto.YandexDiskTokenExpiresOn.Value;
                if (updateUserDto.YandexDiskStatus.HasValue)
                    user.YandexDiskStatus = updateUserDto.YandexDiskStatus.Value;

                if (!string.IsNullOrEmpty(updateUserDto.DropboxToken))
                    user.DropboxToken = updateUserDto.DropboxToken;
                if (updateUserDto.DropboxTokenExpiresOn.HasValue)
                    user.DropboxTokenExpiresOn = updateUserDto.DropboxTokenExpiresOn.Value;
                if (updateUserDto.DropboxStatus.HasValue)
                    user.DropboxStatus = updateUserDto.DropboxStatus.Value;

                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();
        }
    }
}
