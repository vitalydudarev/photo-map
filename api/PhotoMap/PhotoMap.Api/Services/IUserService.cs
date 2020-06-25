using System.Collections.Generic;

namespace PhotoMap.Api.Services
{
    public interface IUserService
    {
        IEnumerable<string> GetUserFiles();
    }
}
