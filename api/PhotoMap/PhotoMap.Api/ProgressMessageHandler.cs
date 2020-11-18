using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Api.Hubs;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Api
{
    public class ProgressMessageHandler : CommandHandler<ProgressMessage>
    {
        private readonly YandexDiskHub _yandexDiskHub;

        public ProgressMessageHandler(YandexDiskHub yandexDiskHub)
        {
            _yandexDiskHub = yandexDiskHub;
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
                // implement
            }
        }
    }

    public class Progress
    {
        public int Processed { get; set; }
        public int Total { get; set; }
    }
}
