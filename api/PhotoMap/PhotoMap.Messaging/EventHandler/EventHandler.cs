using System;
using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Messaging.Events;

namespace PhotoMap.Messaging.EventHandler
{
    public abstract class EventHandler<T> : IEventHandler where T : EventBase
    {
        public Type Type => typeof(T);

        public abstract Task HandleAsync(EventBase @event, CancellationToken cancellationToken);
    }
}
