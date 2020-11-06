using PhotoMap.Worker.Models;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IDropboxDownloadStateService
    {
        DropboxData GetData(string accountId);
        void SaveData(DropboxData data);
    }
}
