using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Common.Commands;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Api.Handlers
{
    public class ImageConvertedHandler : CommandHandler<ImageConverted>
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

        public override Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is ImageConverted imageConverted)
            {
                _convertedImageHolder.Add(imageConverted.Id, imageConverted.FileContents);
                _logger.LogInformation("Converted image for {Id} received", imageConverted.Id);
            }

            return Task.CompletedTask;
        }
    }
}
