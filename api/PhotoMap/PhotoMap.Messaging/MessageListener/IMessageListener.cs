using System.Threading;

namespace PhotoMap.Messaging.MessageListener
{
    public interface IMessageListener
    {
        void InitializeConnection();
        void Listen(CancellationToken cancellationToken);
    }
}
