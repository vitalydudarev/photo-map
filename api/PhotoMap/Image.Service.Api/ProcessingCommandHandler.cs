using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GraphicsLibrary;
using GraphicsLibrary.Exif;
using Image.Service.Services.StorageService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoMap.Common.Commands;
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

                ResultsCommand resultsCommand;

                try
                {
                    resultsCommand = await ProcessImageAsync(fileContents, processingCommand);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to process image {processingCommand.FileName}: {e.Message}");
                    throw;
                }

                _messageSender.Send(resultsCommand);
            }
        }

        private async Task<ResultsCommand> ProcessImageAsync(byte[] fileContents, ProcessingCommand processingCommand)
        {
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

            DateTime? dateTimeTaken = null;
            string exifString = null;
            double? longitude = null;
            double? latitude = null;

            var exifExtractor = new ExifExtractor();

            var exif = exifExtractor.GetDataAsync(fileContents);
            if (exif != null)
            {
                dateTimeTaken = GetDate(exif);

                var gps = exif.Gps;
                if (gps != null)
                {
                    latitude = gps.Latitude != null && gps.LatitudeRef != null
                        ? GpsHelper.ConvertLatitude(gps.Latitude, gps.LatitudeRef)
                        : (double?) null;
                    longitude = gps.Longitude != null && gps.LongitudeRef != null
                        ? GpsHelper.ConvertLongitude(gps.Longitude, gps.LongitudeRef)
                        : (double?) null;
                }

                exifString = JsonConvert.SerializeObject(exif);
            }

            return new ResultsCommand
            {
                UserId = processingCommand.UserId,
                FileId = processingCommand.DeleteAfterProcessing ? (long?) null : processingCommand.FileId,
                FileName = processingCommand.FileName,
                FileSource = processingCommand.FileSource,
                Thumbs = sizeFileIdMap,
                PhotoUrl = processingCommand.FileUrl,
                Path = processingCommand.Path,
                FileCreatedOn = processingCommand.FileCreatedOn,
                PhotoTakenOn = dateTimeTaken,
                ExifString = exifString,
                Latitude = latitude,
                Longitude = longitude
            };
        }

        private static DateTime? GetDate(ExifData exif)
        {
            return exif.Gps?.DateTimeStamp?.ToUniversalTime() ?? exif.ExifSubIfd?.DateTimeOriginal?.ToUniversalTime();
        }
    }
}
