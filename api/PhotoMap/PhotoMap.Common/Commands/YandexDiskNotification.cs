using PhotoMap.Messaging.Commands;

namespace PhotoMap.Common.Commands
{
    public class YandexDiskNotification : CommandBase
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public bool HasError { get; set; }
        public ProcessingStatus Status { get; set; }
    }
}
