using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphicsLibrary.Exif;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PhotoMap.Api.Database.Entities;
using PhotoMap.Api.Database.Services;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Api
{
    public class ResultsCommandHandler : CommandHandler<ResultsCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ResultsCommandHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is ResultsCommand resultsCommand)
            {
                var thumbs = resultsCommand.ThumbsSizes.OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
                var thumbSmall = thumbs.FirstOrDefault().Value;
                var thumbLarge = thumbs.LastOrDefault().Value;

                var photoEntity = new Photo
                {
                    UserId = resultsCommand.UserId,
                    PhotoUrl = resultsCommand.PhotoUrl,
                    HasExternalPhotoUrl = false,
                    PhotoFileId = resultsCommand.FileId,
                    FileName = resultsCommand.FileName,
                    Source = resultsCommand.FileSource,
                    ThumbnailSmallFileId = thumbSmall,
                    ThumbnailLargeFileId = thumbLarge,
                    Path = resultsCommand.Path
                };

                if (resultsCommand.Exif != null)
                {
                    var date = GetDate(resultsCommand.Exif);

                    photoEntity.DateTimeTaken = date ?? DateTime.UtcNow;

                    var gps = resultsCommand.Exif.Gps;
                    if (gps != null)
                    {
                        photoEntity.HasGps = true;
                        photoEntity.Latitude =
                            (gps.Latitude != null && gps.LatitudeRef != null)
                                ? ConvertLatitude(gps.Latitude, gps.LatitudeRef)
                                : (double?) null;
                        photoEntity.Longitude =
                            (gps.Longitude != null && gps.LongitudeRef != null)
                                ? ConvertLatitude(gps.Longitude, gps.LongitudeRef)
                                : (double?) null;
                    }

                    photoEntity.ExifString = JsonConvert.SerializeObject(resultsCommand.Exif);
                }

                var scope = _serviceScopeFactory.CreateScope();
                var photoService = scope.ServiceProvider.GetService<IPhotoService>();
                await photoService.AddAsync(photoEntity);
            }
        }

        private static DateTime? GetDate(ExifData exif)
        {
            if (exif != null)
            {
                if (exif.Gps?.DateTimeStamp != null)
                    return exif.Gps.DateTimeStamp.Value.ToUniversalTime();

                if (exif.Ifd?.DateTimeOriginal != null)
                    return exif.Ifd.DateTimeOriginal.Value.ToUniversalTime();
            }

            return null;
        }

        private double ConvertLatitude(LatLng latLng, string latitudeRef)
        {
            int multiplier = latitudeRef == "S" ? -1 : 1;
            return multiplier * latLng.Degrees + latLng.Minutes / 60 + latLng.Seconds / 3600;
        }

        private double ConvertLongitude(LatLng latLng, string longitudeRef)
        {
            int multiplier = longitudeRef == "W" ? -1 : 1;
            return multiplier * latLng.Degrees + latLng.Minutes / 60 + latLng.Seconds / 3600;
        }
    }
}
