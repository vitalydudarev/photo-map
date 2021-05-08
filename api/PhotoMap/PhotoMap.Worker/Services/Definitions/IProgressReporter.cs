using IUserIdentifier = PhotoMap.Worker.Models.IUserIdentifier;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IProgressReporter
    {
        void Report(IUserIdentifier userIdentifier, int processed, int total);
    }
}
