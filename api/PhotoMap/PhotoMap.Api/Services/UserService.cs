using System.Collections.Generic;

namespace PhotoMap.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IImageService _imageService;

        public UserService(IImageService imageService)
        {
            _imageService = imageService;
        }

        public IEnumerable<string> GetUserFiles()
        {
            return _imageService.GetFiles();
        }
    }
}
