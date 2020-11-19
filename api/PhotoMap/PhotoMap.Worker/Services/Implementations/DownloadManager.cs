using System.Collections.Generic;
using PhotoMap.Common.Models;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Services.Implementations
{
    public class DownloadManager : IDownloadManager
    {
        private readonly Dictionary<string, StoppingAction> _map = new Dictionary<string, StoppingAction>();

        public void Add(IUserIdentifier userIdentifier, StoppingAction stoppingAction)
        {
            _map.Add(userIdentifier.GetKey(), stoppingAction);
        }

        public void Remove(IUserIdentifier userIdentifier)
        {
            var identifier = userIdentifier.GetKey();

            if (_map.TryGetValue(identifier, out var stoppingAction))
            {
                stoppingAction.IsStopRequested = true;
                _map.Remove(identifier);
            }
        }
    }
}
