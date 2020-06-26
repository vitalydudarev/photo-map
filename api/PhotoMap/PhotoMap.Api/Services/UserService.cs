using System.Collections.Generic;

namespace PhotoMap.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IThumbnailService _thumbnailService;

        public UserService(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        public IEnumerable<string> GetUserFiles()
        {
            return _thumbnailService.GetFiles();
        }
    }
}
