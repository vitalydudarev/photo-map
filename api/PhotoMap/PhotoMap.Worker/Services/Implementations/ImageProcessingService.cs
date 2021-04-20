using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhotoMap.Worker.Helpers;
using PhotoMap.Worker.Models;
using PhotoMap.Worker.Models.Image;
using PhotoMap.Worker.Services.Definitions;
using PhotoMap.Worker.Settings;

namespace PhotoMap.Worker.Services.Implementations
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly ILogger<ImageProcessingService> _logger;
        private readonly ImageProcessingSettings _imageProcessingSettings;
        private readonly IStorageService _storageService;

        public ImageProcessingService(
            ILogger<ImageProcessingService> logger,
            IOptions<ImageProcessingSettings> imageProcessingOptions,
            IStorageService storageService)
        {
            _logger = logger;
            _imageProcessingSettings = imageProcessingOptions.Value;
            _storageService = storageService;
        }

        public async Task<ProcessedDownloadedFile> ProcessImageAsync(DownloadedFile downloadedFile)
        {
            var fileContents = await _storageService.GetFileAsync(downloadedFile.FileId);
            using var imageProcessor = new ImageProcessor(fileContents);
            imageProcessor.Rotate();

            var relativeFilePath = downloadedFile.RelativeFilePath;
            var directory = Path.GetDirectoryName(relativeFilePath);
            var extension = Path.GetExtension(relativeFilePath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(relativeFilePath);

            var sizeFileIdMap = new Dictionary<int, long>();

            foreach (var size in _imageProcessingSettings.Sizes)
            {
                var fileName = $"{fileNameWithoutExtension}_{size}{extension}";
                var path = Path.Combine(directory, "thumbs", fileName);

                imageProcessor.Crop(size);
                var bytes = imageProcessor.GetImageBytes();

                var savedFile = await _storageService.SaveFileAsync(path, bytes);

                sizeFileIdMap.Add(size, savedFile.Id);
            }

            if (_imageProcessingSettings.DeleteAfterProcessing)
                await _storageService.DeleteFileAsync(downloadedFile.FileId);

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

            return new ProcessedDownloadedFile
            {
                FileId = _imageProcessingSettings.DeleteAfterProcessing ? (long?) null : downloadedFile.FileId,
                FileName = downloadedFile.FileName,
                FileSource = downloadedFile.FileSource,
                Thumbs = sizeFileIdMap,
                PhotoUrl = downloadedFile.FileUrl,
                Path = downloadedFile.Path,
                FileCreatedOn = downloadedFile.FileCreatedOn,
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
