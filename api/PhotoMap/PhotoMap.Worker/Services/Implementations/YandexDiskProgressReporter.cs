using PhotoMap.Common.Commands;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Services.Implementations
{
    public class YandexDiskProgressReporter : IYandexDiskProgressReporter
    {
        private readonly IMessageSender2 _messageSender;

        public YandexDiskProgressReporter(IMessageSender2 messageSender)
        {
            _messageSender = messageSender;
        }

        public void Report(int userId, int processed, int total)
        {
            var message = new ProgressMessage
            {
                UserId = userId,
                Processed = processed,
                Total = total
            };

            _messageSender.Send(message, Constants.PhotoMapApi);
        }
    }
}
