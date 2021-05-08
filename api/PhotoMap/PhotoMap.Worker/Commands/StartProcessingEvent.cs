using PhotoMap.Messaging.Events;
using IUserIdentifier = PhotoMap.Worker.Models.IUserIdentifier;

namespace PhotoMap.Worker.Commands
{
    public class StartProcessingEvent : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public string Token { get; set; }
    }
}
