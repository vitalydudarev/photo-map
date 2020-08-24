using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Yandex.Disk.Worker
{
    public class YandexDiskService : IYandexDiskService
    {
        private const string FileName = "yandex-disk-data.json";
        private readonly Dictionary<int, YandexDiskData> _map = new Dictionary<int, YandexDiskData>();
        private readonly ILogger<IYandexDiskService> _logger;

        public YandexDiskService(ILogger<IYandexDiskService> logger)
        {
            _logger = logger;

            _logger.LogInformation("Directory: " + Directory.GetCurrentDirectory());

            if (File.Exists(FileName))
            {
                var fileContents = File.ReadAllText(FileName);
                var list = JsonConvert.DeserializeObject<List<YandexDiskData>>(fileContents);
                _map = list.ToDictionary(a => a.UserId, b => b);
            }
        }

        public YandexDiskData GetData(int userId)
        {
            return _map.TryGetValue(userId, out var data) ? data : null;
        }

        public void SaveData(YandexDiskData data)
        {
            if (!_map.TryGetValue(data.UserId, out _))
            {
                data.Started = DateTimeOffset.UtcNow;
                _map.Add(data.UserId, data);
            }

            var values = _map.Values.ToList();

            var fileContents = JsonConvert.SerializeObject(values);
            File.WriteAllText(FileName, fileContents);

            _logger.LogInformation("Directory: " + Directory.GetCurrentDirectory());
            _logger.LogInformation("Saved");
        }
    }
}