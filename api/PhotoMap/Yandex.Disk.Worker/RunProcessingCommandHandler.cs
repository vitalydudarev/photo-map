using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Messaging;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using Yandex.Disk.Worker.Services;

namespace Yandex.Disk.Worker
{
    public class RunProcessingCommandHandler : CommandHandler<RunProcessingCommand>
    {
        private readonly IYandexDiskDownloadService _yandexDiskDownloadService;

        public RunProcessingCommandHandler(IYandexDiskDownloadService yandexDiskDownloadService)
        {
            _yandexDiskDownloadService = yandexDiskDownloadService;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is RunProcessingCommand runProcessingCommand)
            {
                await foreach (var file in _yandexDiskDownloadService.DownloadFilesAsync(runProcessingCommand.Token, cancellationToken))
                {

                }
            }
        }
    }
}
