namespace PhotoMap.Worker.Services.Definitions
{
    public interface IYandexDiskDownloadServiceManager
    {
        void Add(int userId, StoppingAction stoppingAction);
        void Remove(int userId);
    }
}
