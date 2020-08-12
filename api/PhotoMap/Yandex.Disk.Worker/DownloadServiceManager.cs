using System.Collections.Generic;

namespace Yandex.Disk.Worker
{
    public class DownloadServiceManager
    {
        private readonly Dictionary<int, StoppingAction> _map = new Dictionary<int, StoppingAction>();

        public void Start(int userId, StoppingAction stoppingAction)
        {
            _map.Add(userId, stoppingAction);
        }

        public void Stop(int userId)
        {
            if (_map.TryGetValue(userId, out var stoppingAction))
                stoppingAction.IsStopRequested = true;
        }
    }
}
