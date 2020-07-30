using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RunProcessingCommandHandler> _logger;
        private readonly IMessageSender _messageSender;
        private readonly ImageProcessingSettings _imageProcessingSettings;

        public RunProcessingCommandHandler(
            IServiceScopeFactory serviceScopeFactory,
            IMessageSender messageSender,
            IOptions<ImageProcessingSettings> imageProcessingOptions,
            ILogger<RunProcessingCommandHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageSender = messageSender;
            _imageProcessingSettings = imageProcessingOptions.Value;
            _logger = logger;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is RunProcessingCommand runProcessingCommand)
            {
                var scope = _serviceScopeFactory.CreateScope();
                var yandexDiskDownloadService = scope.ServiceProvider.GetService<IYandexDiskDownloadService>();

                await foreach (var file in yandexDiskDownloadService.DownloadFilesAsync(runProcessingCommand.Token, cancellationToken))
                {
                    var processingCommand = new ProcessingCommand
                    {
                        UserId = runProcessingCommand.UserId,
                        FileName = file.Name,
                        FileId = file.StorageFileId,
                        FileUrl = file.FileUrl,
                        FileSource = "Yandex.Disk",
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
