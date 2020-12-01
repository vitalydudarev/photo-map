using PhotoMap.Common.Models;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Common.Commands
{
    public class StartProcessingCommand : CommandBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public string Token { get; set; }
    }
}
