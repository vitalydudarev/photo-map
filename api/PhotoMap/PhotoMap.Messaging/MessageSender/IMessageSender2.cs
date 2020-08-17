using PhotoMap.Messaging.Commands;

namespace PhotoMap.Messaging.MessageSender
{
    public interface IMessageSender2
    {
        void Send(CommandBase commandBase, string consumerApi);
    }
}
