using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace Yandex.Disk.Worker
{
    public class StopProcessingCommandHandler : CommandHandler<StopProcessingCommand>
    {
        private readonly DownloadServiceManager _downloadServiceManager;

        public StopProcessingCommandHandler(DownloadServiceManager downloadServiceManager)
        {
            _downloadServiceManager = downloadServiceManager;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is StopProcessingCommand stopProcessingCommand)
            {
                _downloadServiceManager.Stop(stopProcessingCommand.UserId);
            }
        }
    }
}
