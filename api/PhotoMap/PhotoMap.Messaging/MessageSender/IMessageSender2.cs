using PhotoMap.Messaging.Events;

namespace PhotoMap.Messaging.MessageSender
{
    public interface IMessageSender2
    {
        void Send(EventBase eventBase, string consumerApi);
    }
}
