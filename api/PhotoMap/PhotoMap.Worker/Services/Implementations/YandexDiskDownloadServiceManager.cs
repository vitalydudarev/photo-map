using System.Collections.Generic;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Services.Implementations
{
    public class YandexDiskDownloadServiceManager : IYandexDiskDownloadServiceManager
    {
        private readonly Dictionary<int, StoppingAction> _map = new Dictionary<int, StoppingAction>();

        public void Add(int userId, StoppingAction stoppingAction)
        {
            _map.Add(userId, stoppingAction);
        }

        public void Remove(int userId)
        {
            if (_map.TryGetValue(userId, out var stoppingAction))
            {
                stoppingAction.IsStopRequested = true;
                _map.Remove(userId);
            }
        }
    }
}
