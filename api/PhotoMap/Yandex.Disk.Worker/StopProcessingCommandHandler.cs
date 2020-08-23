using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;

namespace Yandex.Disk.Worker
{
    public class StopProcessingCommandHandler : CommandHandler<StopProcessingCommand>
    {
        private readonly IMessageSender2 _messageSender;
        private readonly DownloadServiceManager _downloadServiceManager;

        public StopProcessingCommandHandler(
            IMessageSender2 messageSender,
            DownloadServiceManager downloadServiceManager)
        {
            _messageSender = messageSender;
            _downloadServiceManager = downloadServiceManager;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is StopProcessingCommand stopProcessingCommand)
            {
                _downloadServiceManager.Remove(stopProcessingCommand.UserId);

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
