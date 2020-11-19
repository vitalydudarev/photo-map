using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotoMap.Common.Commands;
using PhotoMap.Common.Models;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Models;
using PhotoMap.Worker.Services.Definitions;
using PhotoMap.Worker.Settings;

namespace PhotoMap.Worker.Handlers
{
    public class StartProcessingCommandHandler : CommandHandler<StartProcessingCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<StartProcessingCommandHandler> _logger;
        private readonly IMessageSender2 _messageSender;
        private readonly ImageProcessingSettings _imageProcessingSettings;
        private readonly IDownloadManager _downloadManager;

        public StartProcessingCommandHandler(
            IServiceScopeFactory serviceScopeFactory,
            IMessageSender2 messageSender,
            IOptions<ImageProcessingSettings> imageProcessingOptions,
            IDownloadManager downloadManager,
            ILogger<StartProcessingCommandHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageSender = messageSender;
            _imageProcessingSettings = imageProcessingOptions.Value;
            _downloadManager = downloadManager;
            _logger = logger;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is StartProcessingCommand startProcessingCommand)
            {
                var userIdentifier = startProcessingCommand.UserIdentifier;
                if (userIdentifier is YandexDiskUserIdentifier)
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var yandexDiskDownloadService = scope.ServiceProvider.GetService<IYandexDiskDownloadService>();

                    var stoppingAction = new StoppingAction();
                    _downloadManager.Add(userIdentifier, stoppingAction);

                    var startedNotification = CreateNotification(userIdentifier, ProcessingStatus.Running);

                    _messageSender.Send(startedNotification, Constants.PhotoMapApi);

                    try
                    {
                        await foreach (var file in yandexDiskDownloadService.DownloadFilesAsync(userIdentifier,
                            startProcessingCommand.Token, cancellationToken, stoppingAction))
                        {
                            var processingCommand = CreateProcessingCommand(startProcessingCommand, file);

                            _messageSender.Send(processingCommand, Constants.ImageServiceApi);
                        }
                    }
                    catch (YandexDiskException e)
                    {
                        _logger.LogError(e.Message);

                        var stoppedNotification =
                            CreateNotification(userIdentifier, ProcessingStatus.Stopped, true, e.Message);

                        _messageSender.Send(stoppedNotification, Constants.PhotoMapApi);
                    }

                    _downloadManager.Remove(userIdentifier);

                    var finishedNotification = CreateNotification(userIdentifier, ProcessingStatus.Finished);

                    _messageSender.Send(finishedNotification, Constants.PhotoMapApi);

                    _logger.LogInformation("Processing finished.");
                }
            }
        }

        private ProcessingCommand CreateProcessingCommand(
            StartProcessingCommand startProcessingCommand,
            YandexDiskFileKey file)
        {
            return new ProcessingCommand
            {
                UserIdentifier = startProcessingCommand.UserIdentifier,
                FileName = file.ResourceName,
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

        private Notification CreateNotification(IUserIdentifier userIdentifier, ProcessingStatus status,
            bool hasError = false, string message = null)
        {
            return new Notification
            {
                UserIdentifier = userIdentifier,
                Status = status,
                HasError = hasError,
                Message = message
            };
        }
    }
}
