using PhotoMap.Common.Models;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IDownloadManager
    {
        void Add(IUserIdentifier userIdentifier, StoppingAction stoppingAction);
        void Remove(IUserIdentifier userIdentifier);
    }
}
