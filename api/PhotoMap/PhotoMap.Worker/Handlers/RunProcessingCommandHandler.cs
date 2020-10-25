using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Models;
using PhotoMap.Worker.Services;
using PhotoMap.Worker.Services.Definitions;
using PhotoMap.Worker.Services.Implementations;
using PhotoMap.Worker.Settings;

namespace PhotoMap.Worker.Handlers
{
    public class RunProcessingCommandHandler : CommandHandler<RunProcessingCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RunProcessingCommandHandler> _logger;
        private readonly IMessageSender2 _messageSender;
        private readonly ImageProcessingSettings _imageProcessingSettings;
        private readonly YandexDiskDownloadServiceManager _yandexDiskDownloadServiceManager;

        public RunProcessingCommandHandler(
            IServiceScopeFactory serviceScopeFactory,
            IMessageSender2 messageSender,
            IOptions<ImageProcessingSettings> imageProcessingOptions,
            YandexDiskDownloadServiceManager yandexDiskDownloadServiceManager,
            ILogger<RunProcessingCommandHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageSender = messageSender;
            _imageProcessingSettings = imageProcessingOptions.Value;
            _yandexDiskDownloadServiceManager = yandexDiskDownloadServiceManager;
            _logger = logger;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is RunProcessingCommand runProcessingCommand)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var yandexDiskDownloadService = scope.ServiceProvider.GetService<IYandexDiskDownloadService>();

                var stoppingAction = new StoppingAction();
                _yandexDiskDownloadServiceManager.Add(runProcessingCommand.UserId, stoppingAction);

                var startedNotification = new YandexDiskNotification
                {
                    UserId = runProcessingCommand.UserId,
                    Status = PhotoMap.Messaging.Commands.YandexDiskStatus.Running
                };

                _messageSender.Send(startedNotification, Constants.PhotoMapApi);

                try
                {
                    await foreach (var file in yandexDiskDownloadService.DownloadFilesAsync(runProcessingCommand.UserId,
                        runProcessingCommand.Token, cancellationToken, stoppingAction))
                    {
                        var processingCommand = CreateProcessingCommand(runProcessingCommand, file);

                        _messageSender.Send(processingCommand, Constants.ImageServiceApi);
                    }
                }
                catch (YandexDiskException e)
                {
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

                _yandexDiskDownloadServiceManager.Remove(runProcessingCommand.UserId);

                var notification1 = new YandexDiskNotification
                {
                    UserId = runProcessingCommand.UserId,
                    Status = PhotoMap.Messaging.Commands.YandexDiskStatus.Finished
                };

                _messageSender.Send(notification1, Constants.PhotoMapApi);

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
                RelativeFilePath = file.RelativeFilePath,
                FileCreatedOn = file.CreatedOn
            };
        }
    }
}
