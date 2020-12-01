using PhotoMap.Worker.Models;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IYandexDiskDownloadStateService
    {
        YandexDiskData GetData(int userId);

        void SaveData(YandexDiskData data);
    }
}
