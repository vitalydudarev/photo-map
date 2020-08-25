namespace Yandex.Disk.Worker
{
    public interface IProgressReporter
    {
        void Report(int userId, int processed, int total);
    }
}
