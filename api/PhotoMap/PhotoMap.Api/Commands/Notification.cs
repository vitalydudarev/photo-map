using PhotoMap.Messaging.Events;
using IUserIdentifier = PhotoMap.Api.Models.IUserIdentifier;

namespace PhotoMap.Api.Commands
{
    public class Notification : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }
        public string Message { get; set; }
        public bool HasError { get; set; }
        public ProcessingStatus Status { get; set; }
    }
}
