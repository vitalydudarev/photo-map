namespace PhotoMap.Worker.Services.Definitions
{
    public interface IProgressReporter
    {
        void Report(int userId, int processed, int total);
    }
}
