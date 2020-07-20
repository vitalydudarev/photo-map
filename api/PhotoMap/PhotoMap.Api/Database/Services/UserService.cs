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
                    YandexDiskToken = addUserDto.AccessToken,
                    YandexDiskTokenExpiresOn = DateTimeOffset.UtcNow.AddSeconds(addUserDto.TokenExpiresIn)
                };
                await _context.Users.AddAsync(user);
            }
            else
            {
                user.YandexDiskToken = addUserDto.AccessToken;
                user.YandexDiskTokenExpiresOn = DateTimeOffset.UtcNow.AddSeconds(addUserDto.TokenExpiresIn);

                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();
        }
    }
}
