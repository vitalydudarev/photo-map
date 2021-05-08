using System.Collections.Generic;
using System.Threading;
using PhotoMap.Worker.Models;
using IUserIdentifier = PhotoMap.Worker.Models.IUserIdentifier;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IDropboxDownloadService
    {
        IAsyncEnumerable<DropboxFileInfo> DownloadAsync(
            IUserIdentifier userIdentifier,
            string apiToken,
            StoppingAction stoppingAction,
            CancellationToken cancellationToken);
    }
}
