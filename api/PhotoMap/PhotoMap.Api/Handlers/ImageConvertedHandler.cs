using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Messaging.EventHandler;
using PhotoMap.Messaging.Events;
using ImageConverted = PhotoMap.Api.Commands.ImageConverted;

namespace PhotoMap.Api.Handlers
{
    public class ImageConvertedHandler : EventHandler<ImageConverted>
    {
        private readonly ILogger<ImageConvertedHandler> _logger;
        private readonly IConvertedImageHolder _convertedImageHolder;

        public ImageConvertedHandler(
            IConvertedImageHolder convertedImageHolder,
            ILogger<ImageConvertedHandler> logger)
        {
            _convertedImageHolder = convertedImageHolder;
            _logger = logger;
        }

        public override Task HandleAsync(EventBase @event, CancellationToken cancellationToken)
        {
            if (@event is ImageConverted imageConverted)
            {
                _convertedImageHolder.Add(imageConverted.Id, imageConverted.FileContents);
                _logger.LogInformation("Converted image for {Id} received", imageConverted.Id);
            }

            return Task.CompletedTask;
        }
    }
}
