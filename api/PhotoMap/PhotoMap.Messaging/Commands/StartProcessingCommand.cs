using System;

namespace PhotoMap.Messaging.Commands
{
    public class StartProcessingCommand : CommandBase
    {
        public int UserId { get; set; }

        public string Token { get; set; }
    }
}
