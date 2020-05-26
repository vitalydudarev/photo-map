using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Yandex.Disk.Api.Client;
using Yandex.Disk.Api.Client.Models;
using Yandex.Disk.Service.Api.Models;

namespace Yandex.Disk.Service.Api.Services
{
    public class YandexDiskDownloadService
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly ApiClient _apiClient;
        // private readonly IStorage _storage;
        // private readonly IProgress<> _progress;
        private readonly ILogger<YandexDiskDownloadService> _logger;

        public YandexDiskDownloadService(
            ILogger<YandexDiskDownloadService> logger, 
            string accessToken/*, 
            IStorage storage, 
            IProgress progress*/)
        {
            _logger = logger;
            // _storage = storage;
            _apiClient = new ApiClient(accessToken, Client);
            // _progress = progress;
        }
        
        public async IAsyncEnumerable<YandexDiskFileKey> DownloadFiles([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var disk = await _apiClient.GetDiskAsync(cancellationToken);
            _logger.LogInformation("GetDisk");

            const int limit = 100;
            
            int offset = 0;
            int totalCount = 0;
            int downloadedCount = 0;
            
            while (offset <= totalCount)
            {
                var resource = await _apiClient.GetResourceAsync(disk.SystemFolders.Photostream, cancellationToken, offset, limit);
                _logger.LogInformation("GetResource");
                
                var items = resource.Embedded.Items;
                if (items != null && items.Length > 0)
                {
                    totalCount = resource.Embedded.Total;
                    
                    // var progressStat = new ProgressStat { Total = totalCount };
                    // _progress.SetProgress(progressStat);
                    
                    foreach (var item in items)
                    {
                        var entity = await Download(item, disk);

                        downloadedCount++;
                        // progressStat.Downloaded = downloadedCount;

                        yield return entity;
                    }
                }

                offset += limit;
            }
        }

        private async Task<YandexDiskFileKey> Download(Resource resource, Disk.Api.Client.Models.Disk disk)
        {
            _logger.LogInformation($"Started downloading {resource.Name}.");

            try
            {
                var key = new YandexDiskFileKey(disk.User.Login, disk.User.Uid, resource.Name);
                var bytes = await Client.GetByteArrayAsync(resource.File);
                
                // await _storage.SaveAsync(key.ToString(), bytes);
            
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