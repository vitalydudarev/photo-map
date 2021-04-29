using System.Threading.Tasks;
using PhotoMap.Worker.Models;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IImageProcessingService
    {
        Task<ProcessedDownloadedFile> ProcessImageAsync(DownloadedFileInfo downloadedFile);
    }
}
