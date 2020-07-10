using System;

namespace PhotoMap.Messaging.Commands
{
    public class RunProcessingCommand : CommandBase
    {
        public int UserId { get; set; }

        public string Token { get; set; }
    }
}
