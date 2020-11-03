using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Handlers
{
    public class PauseProcessingCommandHandler : CommandHandler<PauseProcessingCommand>
    {
        private readonly IMessageSender2 _messageSender;
        private readonly IYandexDiskDownloadServiceManager _yandexDiskDownloadServiceManager;

        public PauseProcessingCommandHandler(
            IMessageSender2 messageSender,
            IYandexDiskDownloadServiceManager yandexDiskDownloadServiceManager)
        {
            _messageSender = messageSender;
            _yandexDiskDownloadServiceManager = yandexDiskDownloadServiceManager;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is PauseProcessingCommand pauseProcessingCommand)
            {
                _yandexDiskDownloadServiceManager.Remove(pauseProcessingCommand.UserId);

                var startedNotification = new YandexDiskNotification
                {
                    UserId = pauseProcessingCommand.UserId,
                    Status = ProcessingStatus.Stopped
                };

                _messageSender.Send(startedNotification, Constants.PhotoMapApi);
            }
        }
    }
}
