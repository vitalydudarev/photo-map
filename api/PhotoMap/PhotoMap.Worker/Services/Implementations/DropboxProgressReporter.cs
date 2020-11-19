using PhotoMap.Common.Commands;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Services.Implementations
{
    public class DropboxProgressReporter : IDropboxProgressReporter
    {
        private readonly IMessageSender2 _messageSender;

        public DropboxProgressReporter(IMessageSender2 messageSender)
        {
            _messageSender = messageSender;
        }

        public void Report(string accountId, int processed, int total)
        {
            var command = new DropboxProgressCommand
            {
                AccountId = accountId,
                Processed = processed,
                Total = total
            };

            _messageSender.Send(command, Constants.PhotoMapApi);
        }
    }
}
