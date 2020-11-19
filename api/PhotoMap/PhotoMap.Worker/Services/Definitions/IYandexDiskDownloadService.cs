using System.Collections.Generic;
using System.Threading;
using PhotoMap.Common.Models;
using PhotoMap.Worker.Models;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IYandexDiskDownloadService
    {
        IAsyncEnumerable<YandexDiskFileKey> DownloadFilesAsync(
            IUserIdentifier userIdentifier,
            string accessToken,
            CancellationToken cancellationToken,
            StoppingAction stoppingAction);
    }
}
