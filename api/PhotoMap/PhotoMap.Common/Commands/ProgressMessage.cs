using PhotoMap.Common.Models;
using PhotoMap.Messaging.Events;

namespace PhotoMap.Common.Commands
{
    public class ProgressMessage : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public int Processed { get; set; }

        public int Total { get; set; }
    }
}
