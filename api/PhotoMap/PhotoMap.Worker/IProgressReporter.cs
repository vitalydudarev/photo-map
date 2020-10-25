namespace PhotoMap.Worker
{
    public interface IProgressReporter
    {
        void Report(int userId, int processed, int total);
    }
}
