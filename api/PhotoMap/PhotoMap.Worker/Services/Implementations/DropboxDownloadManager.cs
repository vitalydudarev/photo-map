using System.Collections.Generic;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Services.Implementations
{
    public class DropboxDownloadManager : IDropboxDownloadManager
    {
        private readonly Dictionary<string, StoppingAction> _map = new Dictionary<string, StoppingAction>();

        public void Add(string accountId, StoppingAction stoppingAction)
        {
            _map.Add(accountId, stoppingAction);
        }

        public void Remove(string accountId)
        {
            if (_map.TryGetValue(accountId, out var stoppingAction))
            {
                stoppingAction.IsStopRequested = true;
                _map.Remove(accountId);
            }
        }
    }
}
