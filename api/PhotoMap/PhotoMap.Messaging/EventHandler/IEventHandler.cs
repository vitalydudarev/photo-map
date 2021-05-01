using System;
using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Messaging.Events;

namespace PhotoMap.Messaging.EventHandler
{
    public interface IEventHandler
    {
        Type Type { get; }

        Task HandleAsync(EventBase @event, CancellationToken cancellationToken);
    }
}
