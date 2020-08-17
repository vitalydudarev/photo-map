using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Api.Hubs;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Api
{
    public class YandexDiskNotificationHandler : CommandHandler<YandexDiskNotification>
    {
        private readonly YandexDiskHub _yandexDiskHub;

        public YandexDiskNotificationHandler(YandexDiskHub yandexDiskHub)
        {
            _yandexDiskHub = yandexDiskHub;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is YandexDiskNotification yandexDiskNotification)
            {
                await _yandexDiskHub.SendErrorAsync(yandexDiskNotification.UserId, yandexDiskNotification.Error);
            }
        }
    }
}
