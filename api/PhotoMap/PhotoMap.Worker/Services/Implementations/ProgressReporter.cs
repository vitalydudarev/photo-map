using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Services.Definitions;
using IUserIdentifier = PhotoMap.Worker.Models.IUserIdentifier;
using ProgressMessage = PhotoMap.Worker.Commands.ProgressMessage;

namespace PhotoMap.Worker.Services.Implementations
{
    public class ProgressReporter : IProgressReporter
    {
        private readonly IMessageSender2 _messageSender;

        public ProgressReporter(IMessageSender2 messageSender)
        {
            _messageSender = messageSender;
        }

        public void Report(IUserIdentifier userIdentifier, int processed, int total)
        {
            var message = new ProgressMessage
            {
                UserIdentifier = userIdentifier,
                Processed = processed,
                Total = total
            };

            _messageSender.Send(message, Constants.PhotoMapApi);
        }
    }
}
