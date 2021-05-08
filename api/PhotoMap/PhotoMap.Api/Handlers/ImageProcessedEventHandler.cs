using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoMap.Api.Database.Entities;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Messaging.Events;
using ImageProcessedEvent = PhotoMap.Api.Commands.ImageProcessedEvent;

namespace PhotoMap.Api.Handlers
{
    public class ImageProcessedEventHandler : Messaging.EventHandler.EventHandler<ImageProcessedEvent>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ImageProcessedEventHandler> _logger;

        public ImageProcessedEventHandler(IServiceScopeFactory serviceScopeFactory, ILogger<ImageProcessedEventHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public override async Task HandleAsync(EventBase @event, CancellationToken cancellationToken)
        {
            if (@event is ImageProcessedEvent imageProcessedEvent)
            {
                var scope = _serviceScopeFactory.CreateScope();
                var photoService = scope.ServiceProvider.GetService<IPhotoService>();
                var storageService = scope.ServiceProvider.GetService<IStorageService>();

                var thumbs = imageProcessedEvent.Thumbs.OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
                var thumbSmall = thumbs.FirstOrDefault().Value;
                var thumbLarge = thumbs.LastOrDefault().Value;

                var entity = await photoService.GetByFileNameAsync(imageProcessedEvent.FileName);
                if (entity != null)
                {
                    await storageService.DeleteFileAsync(thumbSmall);
                    await storageService.DeleteFileAsync(thumbLarge);

                    _logger.LogInformation($"File {imageProcessedEvent.FileName} already exists.");

                    return;
                }

                var photoEntity = new Photo
                {
                    UserId = imageProcessedEvent.UserIdentifier.UserId,
                    PhotoFileId = null,
                    FileName = imageProcessedEvent.FileName,
                    Source = imageProcessedEvent.FileSource,
                    ThumbnailSmallFileId = thumbSmall,
                    ThumbnailLargeFileId = thumbLarge,
                    Path = imageProcessedEvent.Path,
                    AddedOn = DateTimeOffset.UtcNow,
                    DateTimeTaken =
                        imageProcessedEvent.PhotoTakenOn ?? (imageProcessedEvent.FileCreatedOn ?? DateTime.UtcNow),
                    ExifString = JsonConvert.SerializeObject(imageProcessedEvent.ExifString),
                    Latitude = imageProcessedEvent.Latitude,
                    Longitude = imageProcessedEvent.Longitude,
                    HasGps = imageProcessedEvent.Latitude.HasValue && imageProcessedEvent.Longitude.HasValue
                };

                await photoService.AddAsync(photoEntity);
            }
        }
    }
}
