using PhotoMap.Worker.Models;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IDropboxDownloadStateService
    {
        DropboxDownloadState GetState(string accountId);
        void SaveState(DropboxDownloadState downloadState);
    }
}
