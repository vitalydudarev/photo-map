namespace PhotoMap.Worker.Services.Definitions
{
    public interface IYandexDiskProgressReporter
    {
        void Report(int userId, int processed, int total);
    }
}
