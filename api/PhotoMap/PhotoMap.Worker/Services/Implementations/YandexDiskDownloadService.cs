using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhotoMap.Common.Models;
using PhotoMap.Worker.Models;
using PhotoMap.Worker.Services.Definitions;
using Yandex.Disk.Api.Client;
using Yandex.Disk.Api.Client.Models;

namespace PhotoMap.Worker.Services.Implementations
{
    public class YandexDiskDownloadService : IYandexDiskDownloadService, IDisposable
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly IProgressReporter _progressReporter;
        private readonly IYandexDiskDownloadStateService _yandexDiskDownloadStateService;
        private readonly IStorageService _storageService;
        private readonly ILogger<YandexDiskDownloadService> _logger;
        private int _currentOffset;
        private int _totalFiles;
        private YandexDiskData _data;

        private const int Limit = 100;

        public YandexDiskDownloadService(
            IProgressReporter progressReporter,
            IYandexDiskDownloadStateService yandexDiskDownloadStateService,
            IStorageService storageService,
            ILogger<YandexDiskDownloadService> logger)
        {
            _progressReporter = progressReporter;
            _yandexDiskDownloadStateService = yandexDiskDownloadStateService;
            _storageService = storageService;
            _logger = logger;
        }

        public async IAsyncEnumerable<YandexDiskFileKey> DownloadFilesAsync(
            IUserIdentifier userIdentifier,
            string accessToken,
            [EnumeratorCancellation] CancellationToken cancellationToken,
            StoppingAction stoppingAction)
        {
            var apiClient = new ApiClient(accessToken, Client);

            _data = _yandexDiskDownloadStateService.GetData(userIdentifier.UserId);
            if (_data == null)
            {
                _data = new YandexDiskData { UserId = userIdentifier.UserId, YandexDiskAccessToken = accessToken };
            }
            else
                _currentOffset = _data.CurrentIndex;

            var disk = await WrapApiCallAsync(() => apiClient.GetDiskAsync(cancellationToken));

            bool firstIteration = true;

            while (_currentOffset <= _totalFiles || firstIteration)
            {
                var resource =
                    await WrapApiCallAsync(() =>
                        apiClient.GetResourceAsync(disk.SystemFolders.Photostream, cancellationToken, _currentOffset, Limit));

                _totalFiles = resource.Embedded.Total;

                var items = resource.Embedded.Items;
                if (items != null && items.Length > 0)
                {
                    foreach (var item in items)
                    {
                        if (cancellationToken.IsCancellationRequested || stoppingAction.IsStopRequested)
                        {
                            _logger.LogInformation("Cancellation requested.");
                            yield break;
                        }

                        if (item.MediaType == "video")
                        {
                            _currentOffset++;
                            continue;
                        }

                        var entity = await DownloadAsync(item, disk);
                        if (entity == null)
                            yield break;

                        _currentOffset++;

                        _progressReporter.Report(userIdentifier, _currentOffset, _totalFiles);

                        yield return entity;
                    }
                }

                firstIteration = false;
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("Yandex.Disk Download Service disposed.");
            _logger.LogInformation("Current offset: " + _currentOffset);

            SaveData();
        }

        private async Task<YandexDiskFileKey> DownloadAsync(Resource resource, Disk disk)
        {
            _logger.LogInformation($"Started downloading {resource.Name}.");

            try
            {
                var bytes = await Client.GetByteArrayAsync(resource.File);
                var filePath = Path.Combine("Yandex.Disk", disk.User.Login, resource.Name);

                var savedFile = await _storageService.SaveFileAsync(filePath, bytes);

                var createdOn = resource.Exif != null ? resource.Exif.DateTime : resource.PhotosliceTime;

                var key = new YandexDiskFileKey(disk.User.Login, disk.User.Uid, resource.Name, filePath, savedFile.Id,
                    resource.File, resource.Path, createdOn);

                _logger.LogInformation($"Finished downloading {resource.Name}.");

                return key;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error downloading {resource.Name}. {e.Message}");
            }

            return null;
        }

        private void SaveData()
        {
            _data.CurrentIndex = _currentOffset;
            _data.TotalPhotos = _totalFiles;

            _yandexDiskDownloadStateService.SaveData(_data);
        }

        private async Task<T> WrapApiCallAsync<T>(Func<Task<T>> apiCall)
        {
            try
            {
                return await apiCall();
            }
            catch (Exception e)
            {
                _logger.LogError("Yandex.Disk: " + e.Message);

                SaveData();

                throw new YandexDiskException(e.Message);
            }
        }
    }
}
