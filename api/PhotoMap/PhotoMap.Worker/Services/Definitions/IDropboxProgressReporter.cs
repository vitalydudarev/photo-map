namespace PhotoMap.Worker.Services.Definitions
{
    public interface IDropboxProgressReporter
    {
        void Report(string accountId, int processed, int total);
    }
}
