using PhotoMap.Messaging.Events;
using IUserIdentifier = PhotoMap.Api.Models.IUserIdentifier;

namespace PhotoMap.Api.Commands
{
    public class StartProcessingEvent : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public string Token { get; set; }
    }
}
