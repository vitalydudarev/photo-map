using PhotoMap.Messaging.Events;
using IUserIdentifier = PhotoMap.Api.Models.IUserIdentifier;

namespace PhotoMap.Api.Commands
{
    public class PauseProcessingEvent : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }
    }
}
