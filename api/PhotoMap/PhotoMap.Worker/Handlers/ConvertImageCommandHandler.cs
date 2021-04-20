using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhotoMap.Common.Commands;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Services.Implementations;

namespace PhotoMap.Worker.Handlers
{
    public class ConvertImageCommandHandler : CommandHandler<ConvertImageCommand>
    {
        private readonly ILogger<ConvertImageCommandHandler> _logger;
        private readonly IMessageSender2 _messageSender;

        public ConvertImageCommandHandler(
            IMessageSender2 messageSender,
            ILogger<ConvertImageCommandHandler> logger)
        {
            _messageSender = messageSender;
            _logger = logger;
        }

        public override Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is ConvertImageCommand convertImageCommand)
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
