namespace PhotoMap.Messaging.Commands
{
    public class DropboxProgressCommand : CommandBase
    {
        public string AccountId { get; set; }

        public int Processed { get; set; }

        public int Total { get; set; }
    }
}
