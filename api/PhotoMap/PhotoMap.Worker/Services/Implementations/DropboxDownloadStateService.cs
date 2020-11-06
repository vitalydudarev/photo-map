using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoMap.Worker.Models;

namespace PhotoMap.Worker.Services.Implementations
{
    public class DropboxDownloadStateService
    {
        private const string FileName = "dropbox-data.json";
        private readonly Dictionary<string, DropboxData> _map = new Dictionary<string, DropboxData>();
        private readonly ILogger<DropboxDownloadStateService> _logger;

        public DropboxDownloadStateService(ILogger<DropboxDownloadStateService> logger)
        {
            _logger = logger;

            _logger.LogInformation("Directory: " + Directory.GetCurrentDirectory());

            if (File.Exists(FileName))
            {
                var fileContents = File.ReadAllText(FileName);
                var list = JsonConvert.DeserializeObject<List<DropboxData>>(fileContents);
                _map = list.ToDictionary(a => a.AccountId, b => b);
            }
        }

        public DropboxData GetData(string accountId)
        {
            return _map.TryGetValue(accountId, out var data) ? data : null;
        }

        public void SaveData(DropboxData data)
        {
            if (!_map.TryGetValue(data.AccountId, out _))
            {
                data.Started = DateTimeOffset.UtcNow;
                _map.Add(data.AccountId, data);
            }

            var values = _map.Values.ToList();

            var fileContents = JsonConvert.SerializeObject(values);
            File.WriteAllText(FileName, fileContents);

            _logger.LogInformation("Directory: " + Directory.GetCurrentDirectory());
            _logger.LogInformation("Saved");
        }
    }
}
