using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoMap.Worker.Models;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Services.Implementations
{
    public class DropboxDownloadStateService : IDropboxDownloadStateService
    {
        private const string FileName = "dropbox-data.json";
        private readonly Dictionary<string, DropboxDownloadState> _map = new Dictionary<string, DropboxDownloadState>();
        private readonly ILogger<DropboxDownloadStateService> _logger;

        public DropboxDownloadStateService(ILogger<DropboxDownloadStateService> logger)
        {
            _logger = logger;

            _logger.LogInformation("Directory: " + Directory.GetCurrentDirectory());

            if (File.Exists(FileName))
            {
                var fileContents = File.ReadAllText(FileName);
                var list = JsonConvert.DeserializeObject<List<DropboxDownloadState>>(fileContents);
                _map = list.ToDictionary(a => a.AccountId, b => b);
            }
        }

        public DropboxDownloadState GetState(string accountId)
        {
            return _map.TryGetValue(accountId, out var data) ? data : null;
        }

        public void SaveState(DropboxDownloadState downloadState)
        {
            if (!_map.TryGetValue(downloadState.AccountId, out _))
            {
                downloadState.Started = DateTimeOffset.UtcNow;
                _map.Add(downloadState.AccountId, downloadState);
            }

            var values = _map.Values.ToList();

            var fileContents = JsonConvert.SerializeObject(values);
            File.WriteAllText(FileName, fileContents);

            _logger.LogInformation("Directory: " + Directory.GetCurrentDirectory());
            _logger.LogInformation("Saved");
        }
    }
}
