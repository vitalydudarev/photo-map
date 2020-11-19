using PhotoMap.Messaging.Commands;

namespace PhotoMap.Common.Commands
{
    public class StartProcessingCommand : CommandBase
    {
        public int UserId { get; set; }

        public string Token { get; set; }
    }
}
