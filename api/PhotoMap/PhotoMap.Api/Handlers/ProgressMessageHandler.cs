using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Api.Hubs;
using PhotoMap.Messaging.EventHandler;
using PhotoMap.Messaging.Events;
using DropboxUserIdentifier = PhotoMap.Api.Models.DropboxUserIdentifier;
using ProgressMessage = PhotoMap.Api.Commands.ProgressMessage;
using YandexDiskUserIdentifier = PhotoMap.Api.Models.YandexDiskUserIdentifier;

namespace PhotoMap.Api.Handlers
{
    public class ProgressMessageHandler : EventHandler<ProgressMessage>
    {
        private readonly YandexDiskHub _yandexDiskHub;
        private readonly DropboxHub _dropboxHub;

        public ProgressMessageHandler(YandexDiskHub yandexDiskHub, DropboxHub dropboxHub)
        {
            _yandexDiskHub = yandexDiskHub;
            _dropboxHub = dropboxHub;
        }

        public override async Task HandleAsync(EventBase @event, CancellationToken cancellationToken)
        {
            if (@event is ProgressMessage progressMessage)
            {
                var userId = progressMessage.UserIdentifier.UserId;
                var progress = new Progress { Processed = progressMessage.Processed, Total = progressMessage.Total };

                if (progressMessage.UserIdentifier is YandexDiskUserIdentifier)
                    await _yandexDiskHub.SendProgressAsync(userId, progress);
                else if (progressMessage.UserIdentifier is DropboxUserIdentifier)
                    await _dropboxHub.SendProgressAsync(userId, progress);
            }
        }
    }
}
