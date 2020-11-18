using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PhotoMap.Api.Handlers;

namespace PhotoMap.Api.Hubs
{
    public class DropboxHub : Hub
    {
        private readonly Dictionary<string, HashSet<string>> _map = new Dictionary<string, HashSet<string>>();

        public void RegisterClient(string accountId)
        {
            var connectionId = Context.ConnectionId;

            if (_map.TryGetValue(accountId, out var connectionsIds))
                connectionsIds.Add(connectionId);
            else
                _map.Add(accountId, new HashSet<string> { connectionId });
        }

        public async Task SendErrorAsync(string accountId, string errorText)
        {
            if (_map.TryGetValue(accountId, out var connectionIds))
            {
                await Clients.Clients(connectionIds.ToList()).SendAsync("DropboxError", errorText);
            }
        }

        public async Task SendProgressAsync(string accountId, Progress processed)
        {
            if (_map.TryGetValue(accountId, out var connectionIds))
            {
                await Clients.Clients(connectionIds.ToList()).SendAsync("DropboxProgress", processed);
            }
        }
    }
}
