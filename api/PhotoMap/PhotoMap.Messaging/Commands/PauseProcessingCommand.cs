namespace PhotoMap.Messaging.Commands
{
    public class PauseProcessingCommand : CommandBase
    {
        public int UserId { get; set; }
    }
}
