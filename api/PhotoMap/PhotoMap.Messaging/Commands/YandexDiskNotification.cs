namespace PhotoMap.Messaging.Commands
{
    public class YandexDiskNotification : CommandBase
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public bool HasError { get; set; }
        public YandexDiskStatus Status { get; set; }
    }
}
