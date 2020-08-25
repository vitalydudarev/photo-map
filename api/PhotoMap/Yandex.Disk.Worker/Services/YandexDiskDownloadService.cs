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
    public class YandexDiskDownloadService : IYandexDiskDownloadService, IDisposable
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly IProgressReporter _progressReporter;
        private readonly IYandexDiskService _yandexDiskService;
        // private readonly IStorage _storage;
        // private readonly IProgress<> _progress;
        private readonly IStorageService _storageService;
        private readonly ILogger<YandexDiskDownloadService> _logger;
        private int _currentOffset;
        private int _totalFiles;
        private YandexDiskData _data;

        public YandexDiskDownloadService(
            IProgressReporter progressReporter,
            IYandexDiskService yandexDiskService,
            IStorageService storageService,
            ILogger<YandexDiskDownloadService> logger)
        {
            _progressReporter = progressReporter;
            _yandexDiskService = yandexDiskService;
            _storageService = storageService;
            _logger = logger;
        }

        public async IAsyncEnumerable<YandexDiskFileKey> DownloadFilesAsync(
            int userId,
            string accessToken,
            [EnumeratorCancellation] CancellationToken cancellationToken,
            StoppingAction stoppingAction)
        {
            bool firstStart = false;

            _data = _yandexDiskService.GetData(userId);
            if (_data == null)
            {
                _data = new YandexDiskData { UserId = userId, YandexDiskAccessToken = accessToken };
                firstStart = true;
            }

            var apiClient = new ApiClient(accessToken, Client);

            var diskResult = await WrapApiCallAsync(() => apiClient.GetDiskAsync(cancellationToken));
            if (diskResult.HasError)
                throw new YandexDiskException(diskResult.Error);

            var disk = diskResult.Result;

            const int limit = 100;

            int offset = firstStart ? 0 : _data.CurrentIndex;
            int totalCount = 0;
            int downloadedCount = 0;
            bool firstIteration = true;

            // int testLimit = 500;
            // totalCount = testLimit;

            _currentOffset = offset;

            while (offset <= totalCount || firstIteration)
            {
                var resourceResult =
                    await WrapApiCallAsync(() =>
                        apiClient.GetResourceAsync(disk.SystemFolders.Photostream, cancellationToken, offset, limit));
                if (resourceResult.HasError)
                    throw new YandexDiskException(resourceResult.Error);

                var resource = resourceResult.Result;
                _totalFiles = resource.Embedded.Total;

                var items = resource.Embedded.Items;
                if (items != null && items.Length > 0)
                {
                    totalCount = resource.Embedded.Total;

                    // var progressStat = new ProgressStat { Total = totalCount };
                    // _progress.SetProgress(progressStat);

                    foreach (var item in items)
                    {
                        if (cancellationToken.IsCancellationRequested || stoppingAction.IsStopRequested)
                        {
                            _logger.LogInformation("Cancellation requested.");
                            yield break;
                        }

                        var entity = await DownloadAsync(item, disk);
                        if (entity == null)
                            yield break;

                        downloadedCount++;

                        _currentOffset++;

                        _progressReporter.Report(userId, _currentOffset, totalCount);

                        // progressStat.Downloaded = downloadedCount;

                        yield return entity;
                    }
                }

                offset += limit;
                firstIteration = false;
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("Download service disposed.");
            _logger.LogInformation("Current offset: " + _currentOffset);

            SaveData();
        }

        private async Task<ApiCallResult<T>> WrapApiCallAsync<T>(Func<Task<T>> apiCall)
        {
            try
            {
                var result = await apiCall();

                return new ApiCallResult<T> { Result = result };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                SaveData();

                return new ApiCallResult<T> { HasError = true, Error = e.Message };
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

                var key = new YandexDiskFileKey(disk.User.Login, disk.User.Uid, resource.Name, filePath, savedFile.Id, resource.File, resource.Path);

                _logger.LogInformation($"Finished downloading {key}.");

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

            _yandexDiskService.SaveData(_data);
        }
    }

    public class ApiCallResult<T>
    {
        public T Result { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }
    }
}
