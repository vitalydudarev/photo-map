using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoMap.Api.Database.Entities;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Common.Commands;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Api.Handlers
{
    public class ResultsCommandHandler : CommandHandler<ResultsCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ResultsCommandHandler> _logger;

        public ResultsCommandHandler(IServiceScopeFactory serviceScopeFactory, ILogger<ResultsCommandHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is ResultsCommand resultsCommand)
            {
                var scope = _serviceScopeFactory.CreateScope();
                var photoService = scope.ServiceProvider.GetService<IPhotoService>();
                var storageService = scope.ServiceProvider.GetService<IStorageService>();

                var thumbs = resultsCommand.Thumbs.OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
                var thumbSmall = thumbs.FirstOrDefault().Value;
                var thumbLarge = thumbs.LastOrDefault().Value;

                var entity = await photoService.GetByFileNameAsync(resultsCommand.FileName);
                if (entity != null)
                {
                    await storageService.DeleteFileAsync(thumbSmall);
                    await storageService.DeleteFileAsync(thumbLarge);

                    if (resultsCommand.FileId.HasValue)
                        await storageService.DeleteFileAsync(resultsCommand.FileId.Value);

                    _logger.LogInformation($"File {resultsCommand.FileName} already exists.");

                    return;
                }

                var photoEntity = new Photo
                {
                    UserId = resultsCommand.UserIdentifier.UserId,
                    PhotoFileId = resultsCommand.FileId,
                    FileName = resultsCommand.FileName,
                    Source = resultsCommand.FileSource,
                    ThumbnailSmallFileId = thumbSmall,
                    ThumbnailLargeFileId = thumbLarge,
                    Path = resultsCommand.Path,
                    AddedOn = DateTimeOffset.UtcNow,
                    DateTimeTaken =
                        resultsCommand.PhotoTakenOn ?? (resultsCommand.FileCreatedOn ?? DateTime.UtcNow),
                    ExifString = JsonConvert.SerializeObject(resultsCommand.ExifString),
                    Latitude = resultsCommand.Latitude,
                    Longitude = resultsCommand.Longitude,
                    HasGps = resultsCommand.Latitude.HasValue && resultsCommand.Longitude.HasValue
                };

                await photoService.AddAsync(photoEntity);
            }
        }
    }
}
