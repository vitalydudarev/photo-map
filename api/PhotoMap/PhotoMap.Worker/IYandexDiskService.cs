namespace PhotoMap.Worker
{
    public interface IYandexDiskService
    {
        YandexDiskData GetData(int userId);

        void SaveData(YandexDiskData data);
    }
}
