using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GraphicsLibrary;
using GraphicsLibrary.Exif;
using Image.Service.Services.StorageService;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace Image.Service
{
    public class ProcessingCommandHandler : CommandHandler<ProcessingCommand>
    {
        private readonly IStorageService _storageService;

        public ProcessingCommandHandler(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is ProcessingCommand processingCommand)
            {
                var fileContents = await _storageService.GetFileAsync(processingCommand.FileId);

                var exifExtractor = new ExifExtractor();
                var exif = exifExtractor.GetDataAsync(fileContents);

                using var imageProcessor = new ImageProcessor(fileContents);
                imageProcessor.Rotate();

                var relativeFilePath = processingCommand.RelativeFilePath;
                var directory = Path.GetDirectoryName(relativeFilePath);
                var extension = Path.GetExtension(relativeFilePath);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(relativeFilePath);

                foreach (var size in processingCommand.Sizes)
                {
                    var fileName = $"{fileNameWithoutExtension}_{size}.{extension}";
                    var path = Path.Combine(directory, "thumbs", fileName);

                    imageProcessor.Crop(size);
                    var bytes = imageProcessor.GetImageBytes();

                    var savedFile = await _storageService.SaveFileAsync(path, bytes);
                }
            }
        }

        private double ConvertLatLng(LatLng latLng)
        {
            return latLng.Degrees + latLng.Minutes / 60 + latLng.Seconds / 3600;
        }
    }
}
