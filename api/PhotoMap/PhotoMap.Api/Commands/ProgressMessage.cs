using PhotoMap.Messaging.Events;
using IUserIdentifier = PhotoMap.Api.Models.IUserIdentifier;

namespace PhotoMap.Api.Commands
{
    public class ProgressMessage : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public int Processed { get; set; }

        public int Total { get; set; }
    }
}
