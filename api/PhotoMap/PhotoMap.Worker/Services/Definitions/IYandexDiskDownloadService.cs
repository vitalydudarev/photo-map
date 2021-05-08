using System.Collections.Generic;
using System.Threading;
using PhotoMap.Worker.Models;
using IUserIdentifier = PhotoMap.Worker.Models.IUserIdentifier;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IYandexDiskDownloadService
    {
        IAsyncEnumerable<YandexDiskFileInfo> DownloadFilesAsync(
            IUserIdentifier userIdentifier,
            string accessToken,
            CancellationToken cancellationToken,
            StoppingAction stoppingAction);
    }
}
