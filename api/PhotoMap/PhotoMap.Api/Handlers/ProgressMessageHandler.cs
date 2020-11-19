using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Api.Hubs;
using PhotoMap.Common.Commands;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Api.Handlers
{
    public class ProgressMessageHandler : CommandHandler<ProgressMessage>
    {
        private readonly YandexDiskHub _yandexDiskHub;
        private readonly DropboxHub _dropboxHub;

        public ProgressMessageHandler(YandexDiskHub yandexDiskHub, DropboxHub dropboxHub)
        {
            _yandexDiskHub = yandexDiskHub;
            _dropboxHub = dropboxHub;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is ProgressMessage progressMessage)
            {
                await _yandexDiskHub.SendProgressAsync(progressMessage.UserId, new Progress
                {
                    Processed = progressMessage.Processed,
                    Total = progressMessage.Total
                });
            }

            if (command is DropboxProgressCommand progressCommand)
            {
                await _dropboxHub.SendProgressAsync(progressCommand.AccountId, new Progress
                {
                    Processed = progressCommand.Processed,
                    Total = progressCommand.Total
                });
            }
        }
    }
}
