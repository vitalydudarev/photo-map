using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using Yandex.Disk.Worker.Models;
using Yandex.Disk.Worker.Services;

namespace Yandex.Disk.Worker
{
    public class RunProcessingCommandHandler : CommandHandler<RunProcessingCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RunProcessingCommandHandler> _logger;
        private readonly IMessageSender2 _messageSender;
        private readonly ImageProcessingSettings _imageProcessingSettings;
        private readonly DownloadServiceManager _downloadServiceManager;

        public RunProcessingCommandHandler(
            IServiceScopeFactory serviceScopeFactory,
            IMessageSender2 messageSender,
            IOptions<ImageProcessingSettings> imageProcessingOptions,
            DownloadServiceManager downloadServiceManager,
            ILogger<RunProcessingCommandHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageSender = messageSender;
            _imageProcessingSettings = imageProcessingOptions.Value;
            _downloadServiceManager = downloadServiceManager;
            _logger = logger;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is RunProcessingCommand runProcessingCommand)
            {
                var scope = _serviceScopeFactory.CreateScope();
                var yandexDiskDownloadService = scope.ServiceProvider.GetService<IYandexDiskDownloadService>();

                var stoppingAction = new StoppingAction();
                _downloadServiceManager.Start(runProcessingCommand.UserId, stoppingAction);

                var startedNotification = new YandexDiskNotification
                {
                    UserId = runProcessingCommand.UserId,
                    Status = PhotoMap.Messaging.Commands.YandexDiskStatus.Running
                };

                _messageSender.Send(startedNotification, Constants.PhotoMapApi);

                try
                {
                    await foreach (var file in yandexDiskDownloadService.DownloadFilesAsync(runProcessingCommand.Token,
                        cancellationToken, stoppingAction))
                    {
                        var processingCommand = CreateProcessingCommand(runProcessingCommand, file);

                        _messageSender.Send(processingCommand, Constants.ImageServiceApi);
                    }
                }
                catch (YandexDiskException e)
                {
                    _downloadServiceManager.Stop(runProcessingCommand.UserId);

                    _logger.LogError(e.Message);

                    var notification = new YandexDiskNotification
                    {
                        Message = e.Message,
                        UserId = runProcessingCommand.UserId,
                        HasError = true,
                        Status = PhotoMap.Messaging.Commands.YandexDiskStatus.Stopped
                    };

                    _messageSender.Send(notification, Constants.PhotoMapApi);
                }

                _logger.LogInformation("Processing finished.");
            }
        }

        private ProcessingCommand CreateProcessingCommand(
            RunProcessingCommand runProcessingCommand,
            YandexDiskFileKey file)
        {
            return new ProcessingCommand
            {
                UserId = runProcessingCommand.UserId,
                FileName = file.Name,
                FileId = file.StorageFileId,
                FileUrl = file.FileUrl,
                Path = file.Path,
                FileSource = "Yandex.Disk",
                DeleteAfterProcessing = _imageProcessingSettings.DeleteAfterProcessing,
                Sizes = _imageProcessingSettings.Sizes,
                RelativeFilePath = file.RelativeFilePath
            };
        }
    }
}
