namespace PhotoMap.Worker.Services.Definitions
{
    public interface IDropboxDownloadManager
    {
        void Add(string accountId, StoppingAction stoppingAction);
        void Remove(string accountId);
    }
}
