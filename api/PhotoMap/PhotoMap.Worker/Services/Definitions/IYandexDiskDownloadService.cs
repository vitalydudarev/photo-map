using System.Collections.Generic;
using System.Threading;
using PhotoMap.Worker.Models;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IYandexDiskDownloadService
    {
        IAsyncEnumerable<YandexDiskFileKey> DownloadFilesAsync(
            int userId,
            string accessToken,
            CancellationToken cancellationToken,
            StoppingAction stoppingAction);
    }
}
