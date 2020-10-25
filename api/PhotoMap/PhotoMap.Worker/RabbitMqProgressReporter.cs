using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;

namespace PhotoMap.Worker
{
    public class RabbitMqProgressReporter : IProgressReporter
    {
        private readonly IMessageSender2 _messageSender;

        public RabbitMqProgressReporter(IMessageSender2 messageSender)
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
