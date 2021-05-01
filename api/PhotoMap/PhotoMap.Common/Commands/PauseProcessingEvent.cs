using PhotoMap.Common.Models;
using PhotoMap.Messaging.Events;

namespace PhotoMap.Common.Commands
{
    public class PauseProcessingEvent : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }
    }
}
