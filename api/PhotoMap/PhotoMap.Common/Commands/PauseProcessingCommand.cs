using PhotoMap.Messaging.Commands;

namespace PhotoMap.Common.Commands
{
    public class PauseProcessingCommand : CommandBase
    {
        public int UserId { get; set; }
    }
}
