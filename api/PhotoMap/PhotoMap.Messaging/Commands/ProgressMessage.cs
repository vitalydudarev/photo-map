namespace PhotoMap.Messaging.Commands
{
    public class ProgressMessage : CommandBase
    {
        public int UserId { get; set; }

        public int Processed { get; set; }

        public int Total { get; set; }
    }
}
