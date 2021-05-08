using PhotoMap.Messaging.Events;
using IUserIdentifier = PhotoMap.Worker.Models.IUserIdentifier;

namespace PhotoMap.Worker.Commands
{
    public class ProgressMessage : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public int Processed { get; set; }

        public int Total { get; set; }
    }
}
