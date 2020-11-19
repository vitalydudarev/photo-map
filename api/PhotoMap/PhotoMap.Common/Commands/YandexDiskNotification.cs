using PhotoMap.Common.Models;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Common.Commands
{
    public class YandexDiskNotification : CommandBase
    {
        public IUserIdentifier UserIdentifier { get; set; }
        public string Message { get; set; }
        public bool HasError { get; set; }
        public ProcessingStatus Status { get; set; }
    }
}
