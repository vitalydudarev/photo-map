using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using PhotoMap.Common.Models;
using PhotoMap.Worker.Models;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IDropboxDownloadService
    {
        IAsyncEnumerable<DropboxFile> DownloadAsync(
            IUserIdentifier userIdentifier,
            string apiToken,
            StoppingAction stoppingAction,
            [EnumeratorCancellation] CancellationToken cancellationToken);
    }
}
