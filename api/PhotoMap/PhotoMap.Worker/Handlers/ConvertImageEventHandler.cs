using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhotoMap.Common.Commands;
using PhotoMap.Messaging.EventHandler;
using PhotoMap.Messaging.Events;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Services.Implementations;

namespace PhotoMap.Worker.Handlers
{
    public class ConvertImageEventHandler : EventHandler<ConvertImageEvent>
    {
        private readonly ILogger<ConvertImageEventHandler> _logger;
        private readonly IMessageSender2 _messageSender;

        public ConvertImageEventHandler(
            IMessageSender2 messageSender,
            ILogger<ConvertImageEventHandler> logger)
        {
            _messageSender = messageSender;
            _logger = logger;
        }

        public override Task HandleAsync(EventBase @event, CancellationToken cancellationToken)
        {
            if (@event is ConvertImageEvent convertImageCommand)
            {
                var imageProcessor = new ImageProcessor(convertImageCommand.FileContents);
                var convertImageBytes = imageProcessor.GetImageBytes();

                var imageConverted = new ImageConverted
                {
                    Id = convertImageCommand.Id,
                    FileContents = convertImageBytes
                };

                _logger.LogInformation("Image for {Id} converted", convertImageCommand.Id);
                _messageSender.Send(imageConverted, Constants.PhotoMapApi);
            }

            return Task.CompletedTask;
        }
    }
}
