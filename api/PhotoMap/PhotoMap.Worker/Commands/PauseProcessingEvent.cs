using PhotoMap.Messaging.Events;
using IUserIdentifier = PhotoMap.Worker.Models.IUserIdentifier;

namespace PhotoMap.Worker.Commands
{
    public class PauseProcessingEvent : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }
    }
}
