using IUserIdentifier = PhotoMap.Worker.Models.IUserIdentifier;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IDownloadManager
    {
        void Add(IUserIdentifier userIdentifier, StoppingAction stoppingAction);
        void Remove(IUserIdentifier userIdentifier);
    }
}
