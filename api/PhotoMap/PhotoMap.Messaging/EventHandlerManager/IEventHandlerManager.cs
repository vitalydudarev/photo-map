using PhotoMap.Messaging.EventHandler;
using PhotoMap.Messaging.Events;

namespace PhotoMap.Messaging.EventHandlerManager
{
    public interface IEventHandlerManager
    {
        IEventHandler GetHandler(EventBase eventBase);
    }
}
