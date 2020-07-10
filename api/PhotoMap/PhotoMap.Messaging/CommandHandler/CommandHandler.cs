using System;
using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Messaging.CommandHandler
{
    public abstract class CommandHandler<T> : ICommandHandler where T : CommandBase
    {
        public Type CommandType => typeof(T);

        public abstract Task HandleAsync(CommandBase command, CancellationToken cancellationToken);
    }
}
