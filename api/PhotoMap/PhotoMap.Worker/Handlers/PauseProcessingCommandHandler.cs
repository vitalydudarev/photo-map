using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Common.Commands;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Handlers
{
    public class PauseProcessingCommandHandler : CommandHandler<PauseProcessingCommand>
    {
        private readonly IMessageSender2 _messageSender;
        private readonly IDownloadManager _downloadManager;

        public PauseProcessingCommandHandler(
            IMessageSender2 messageSender,
            IDownloadManager downloadManager)
        {
            _messageSender = messageSender;
            _downloadManager = downloadManager;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is PauseProcessingCommand pauseProcessingCommand)
            {
                _downloadManager.Remove(pauseProcessingCommand.UserIdentifier);

                var startedNotification = new Notification
                {
                    UserIdentifier = pauseProcessingCommand.UserIdentifier,
                    Status = ProcessingStatus.Stopped
                };

                _messageSender.Send(startedNotification, Constants.PhotoMapApi);
            }
        }
    }
}
