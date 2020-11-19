using PhotoMap.Common.Models;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Common.Commands
{
    public class PauseProcessingCommand : CommandBase
    {
        public IUserIdentifier UserIdentifier { get; set; }
    }
}
