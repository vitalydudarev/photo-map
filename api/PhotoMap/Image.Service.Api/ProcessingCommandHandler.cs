using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GraphicsLibrary;
using Image.Service.Services.StorageService;
using Microsoft.Extensions.Logging;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;

namespace Image.Service
{
    public class ProcessingCommandHandler : CommandHandler<ProcessingCommand>
    {
        private readonly ILogger<ProcessingCommandHandler> _logger;
        private readonly IStorageService _storageService;
        private readonly IMessageSender _messageSender;

        public ProcessingCommandHandler(
            ILogger<ProcessingCommandHandler> logger,
            IStorageService storageService,
            IMessageSender messageSender)
        {
            _logger = logger;
            _storageService = storageService;
            _messageSender = messageSender;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is ProcessingCommand processingCommand)
            {
                byte[] fileContents;

                try
                {
                    fileContents = await _storageService.GetFileAsync(processingCommand.FileId);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to download file {processingCommand.FileId}: {e.Message}");
                    return;
                }

                var exifExtractor = new ExifExtractor();
                var exif = exifExtractor.GetDataAsync(fileContents);

                using var imageProcessor = new ImageProcessor(fileContents);
                imageProcessor.Rotate();

                var relativeFilePath = processingCommand.RelativeFilePath;
                var directory = Path.GetDirectoryName(relativeFilePath);
                var extension = Path.GetExtension(relativeFilePath);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(relativeFilePath);

                var sizeFileIdMap = new Dictionary<int, long>();

                foreach (var size in processingCommand.Sizes)
                {
                    var fileName = $"{fileNameWithoutExtension}_{size}{extension}";
                    var path = Path.Combine(directory, "thumbs", fileName);

                    imageProcessor.Crop(size);
                    var bytes = imageProcessor.GetImageBytes();

                    var savedFile = await _storageService.SaveFileAsync(path, bytes);

                    sizeFileIdMap.Add(size, savedFile.Id);
                }

                if (processingCommand.DeleteAfterProcessing)
                    await _storageService.DeleteFileAsync(processingCommand.FileId);

                var resultsCommand = new ResultsCommand
                {
                    UserId = processingCommand.UserId,
                    FileId = processingCommand.DeleteAfterProcessing ? (long?) null : processingCommand.FileId,
                    FileName = processingCommand.FileName,
                    FileSource = processingCommand.FileSource,
                    Exif = exif,
                    ThumbsSizes = sizeFileIdMap,
                    PhotoUrl = processingCommand.FileUrl,
                    Path = processingCommand.Path,
                    FileCreatedOn = processingCommand.FileCreatedOn
                };

                _messageSender.Send(resultsCommand);
            }
        }
    }
}
