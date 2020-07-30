using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Yandex.Disk.Api.Client;
using Yandex.Disk.Api.Client.Models;
using Yandex.Disk.Worker.Models;
using Yandex.Disk.Worker.Services.External;

namespace Yandex.Disk.Worker.Services
{
    public class YandexDiskDownloadService : IYandexDiskDownloadService
    {
        private static readonly HttpClient Client = new HttpClient();
        // private readonly IStorage _storage;
        // private readonly IProgress<> _progress;
        private readonly IStorageService _storageService;
        private readonly ILogger<YandexDiskDownloadService> _logger;

        public YandexDiskDownloadService(IStorageService storageService, ILogger<YandexDiskDownloadService> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        public async IAsyncEnumerable<YandexDiskFileKey> DownloadFilesAsync(
            string accessToken,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var apiClient = new ApiClient(accessToken, Client);

            var disk = await apiClient.GetDiskAsync(cancellationToken);
            _logger.LogInformation("GetDisk");

            const int limit = 100;

            int offset = 0;
            int totalCount = 0;
            int downloadedCount = 0;

            while (offset <= totalCount)
            {
                var resource =
                    await apiClient.GetResourceAsync(disk.SystemFolders.Photostream, cancellationToken, offset, limit);

                var items = resource.Embedded.Items;
                if (items != null && items.Length > 0)
                {
                    totalCount = resource.Embedded.Total;

                    // var progressStat = new ProgressStat { Total = totalCount };
                    // _progress.SetProgress(progressStat);

                    foreach (var item in items)
                    {
                        var entity = await DownloadAsync(item, disk);

                        downloadedCount++;
                        // progressStat.Downloaded = downloadedCount;

                        yield return entity;
                    }
                }

                offset += limit;
            }
        }

        private async Task<YandexDiskFileKey> DownloadAsync(Resource resource, Disk.Api.Client.Models.Disk disk)
        {
            _logger.LogInformation($"Started downloading {resource.Name}.");

            try
            {
                var bytes = await Client.GetByteArrayAsync(resource.File);
                var filePath = Path.Combine("Yandex.Disk", disk.User.Login, resource.Name);

                var savedFile = await _storageService.SaveFileAsync(filePath, bytes);

                var key = new YandexDiskFileKey(disk.User.Login, disk.User.Uid, resource.Name, filePath, savedFile.Id, resource.File);

                _logger.LogInformation($"Finished downloading {key}.");

                return key;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error downloading {resource.Name}. {e.Message}");
            }

            return null;
        }
    }
}
