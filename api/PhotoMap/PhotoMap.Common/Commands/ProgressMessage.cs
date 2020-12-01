using PhotoMap.Common.Models;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Common.Commands
{
    public class ProgressMessage : CommandBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public int Processed { get; set; }

        public int Total { get; set; }
    }
}
