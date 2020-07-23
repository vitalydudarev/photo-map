using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using Yandex.Disk.Worker.Services;

namespace Yandex.Disk.Worker
{
    public class RunProcessingCommandHandler : CommandHandler<RunProcessingCommand>
    {
        private readonly IYandexDiskDownloadService _yandexDiskDownloadService;
        private readonly ILogger<RunProcessingCommandHandler> _logger;
        private readonly IMessageSender _messageSender;
        private readonly ImageProcessingSettings _imageProcessingSettings;

        public RunProcessingCommandHandler(
            IYandexDiskDownloadService yandexDiskDownloadService,
            IMessageSender messageSender,
            IOptions<ImageProcessingSettings> imageProcessingOptions,
            ILogger<RunProcessingCommandHandler> logger)
        {
            _yandexDiskDownloadService = yandexDiskDownloadService;
            _messageSender = messageSender;
            _imageProcessingSettings = imageProcessingOptions.Value;
            _logger = logger;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is RunProcessingCommand runProcessingCommand)
            {
                await foreach (var file in _yandexDiskDownloadService.DownloadFilesAsync(runProcessingCommand.Token, cancellationToken))
                {
                    var processingCommand = new ProcessingCommand
                    {
                        UserId = runProcessingCommand.UserId,
                        FileId = file.StorageFileId,
                        FileUrl = file.FileUrl,
                        DeleteAfterProcessing = _imageProcessingSettings.DeleteAfterProcessing,
                        Sizes = _imageProcessingSettings.Sizes,
                        RelativeFilePath = file.RelativeFilePath
                    };

                    _messageSender.Send(processingCommand);
                }
            }
        }
    }
}
