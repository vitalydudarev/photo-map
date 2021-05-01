using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Common.Commands;
using PhotoMap.Messaging.EventHandler;
using PhotoMap.Messaging.Events;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Handlers
{
    public class PauseProcessingEventHandler : EventHandler<PauseProcessingEvent>
    {
        private readonly IMessageSender2 _messageSender;
        private readonly IDownloadManager _downloadManager;

        public PauseProcessingEventHandler(
            IMessageSender2 messageSender,
            IDownloadManager downloadManager)
        {
            _messageSender = messageSender;
            _downloadManager = downloadManager;
        }

        public override async Task HandleAsync(EventBase @event, CancellationToken cancellationToken)
        {
            if (@event is PauseProcessingEvent pauseProcessingCommand)
            {
                _downloadManager.Remove(pauseProcessingCommand.UserIdentifier);

                var startedNotification = new Notification
                {
                    UserIdentifier = pauseProcessingCommand.UserIdentifier,
                    Status = ProcessingStatus.NotRunning
                };

                _messageSender.Send(startedNotification, Constants.PhotoMapApi);
            }
        }
    }
}
