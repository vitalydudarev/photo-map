using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PhotoMap.Api.Handlers;

namespace PhotoMap.Api.Hubs
{
    public class YandexDiskHub : Hub
    {
        private readonly Dictionary<int, HashSet<string>> _map = new Dictionary<int, HashSet<string>>();

        public void RegisterClient(int userId)
        {
            var connectionId = Context.ConnectionId;

            if (_map.TryGetValue(userId, out var connectionsIds))
                connectionsIds.Add(connectionId);
            else
                _map.Add(userId, new HashSet<string> { connectionId });
        }

        public async Task SendErrorAsync(int userId, string errorText)
        {
            if (_map.TryGetValue(userId, out var connectionIds))
            {
                await Clients.Clients(connectionIds.ToList()).SendAsync("YandexDiskError", errorText);
            }
        }

        public async Task SendProgressAsync(int userId, Progress processed)
        {
            if (_map.TryGetValue(userId, out var connectionIds))
            {
                await Clients.Clients(connectionIds.ToList()).SendAsync("YandexDiskProgress", processed);
            }
        }
    }
}
