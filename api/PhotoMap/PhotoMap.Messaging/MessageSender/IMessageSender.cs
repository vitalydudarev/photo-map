using PhotoMap.Messaging.Commands;

namespace PhotoMap.Messaging.MessageSender
{
    public interface IMessageSender
    {
        void Send(CommandBase commandBase);
    }
}
