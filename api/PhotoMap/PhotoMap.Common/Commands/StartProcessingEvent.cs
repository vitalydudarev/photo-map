using PhotoMap.Common.Models;
using PhotoMap.Messaging.Events;

namespace PhotoMap.Common.Commands
{
    public class StartProcessingEvent : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public string Token { get; set; }
    }
}
