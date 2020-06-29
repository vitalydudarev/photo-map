using System.Collections.Generic;
using System.Threading;
using Yandex.Disk.Worker.Models;

namespace Yandex.Disk.Worker.Services
{
    public interface IYandexDiskDownloadService
    {
        IAsyncEnumerable<YandexDiskFileKey> DownloadFiles(string accessToken, CancellationToken cancellationToken);
    }
}
