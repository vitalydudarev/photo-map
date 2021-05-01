using PhotoMap.Messaging.Events;

namespace PhotoMap.Messaging.MessageSender
{
    public interface IMessageSender
    {
        void Send(EventBase eventBase);
    }
}
