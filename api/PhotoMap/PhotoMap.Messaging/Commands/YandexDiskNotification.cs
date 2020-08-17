namespace PhotoMap.Messaging.Commands
{
    public class YandexDiskNotification : CommandBase
    {
        public int UserId { get; set; }
        public string Error { get; set; }
    }
}
