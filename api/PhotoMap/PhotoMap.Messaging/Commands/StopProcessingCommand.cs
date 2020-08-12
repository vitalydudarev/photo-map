namespace PhotoMap.Messaging.Commands
{
    public class StopProcessingCommand : CommandBase
    {
        public int UserId { get; set; }
    }
}
