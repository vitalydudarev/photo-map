using System.Threading;

namespace PhotoMap.Messaging.MessageListener
{
    public interface IMessageListener
    {
        void Listen(CancellationToken cancellationToken);
    }
}
