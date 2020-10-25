using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Services.Implementations;

namespace PhotoMap.Worker.Handlers
{
    public class StopProcessingCommandHandler : CommandHandler<StopProcessingCommand>
    {
        private readonly IMessageSender2 _messageSender;
        private readonly YandexDiskDownloadServiceManager _yandexDiskDownloadServiceManager;

        public StopProcessingCommandHandler(
            IMessageSender2 messageSender,
            YandexDiskDownloadServiceManager yandexDiskDownloadServiceManager)
        {
            _messageSender = messageSender;
            _yandexDiskDownloadServiceManager = yandexDiskDownloadServiceManager;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is StopProcessingCommand stopProcessingCommand)
            {
                _yandexDiskDownloadServiceManager.Remove(stopProcessingCommand.UserId);

                var startedNotification = new YandexDiskNotification
                {
                    UserId = stopProcessingCommand.UserId,
                    Status = PhotoMap.Messaging.Commands.YandexDiskStatus.Stopped
                };

                _messageSender.Send(startedNotification, Constants.PhotoMapApi);
            }
        }
    }
}
