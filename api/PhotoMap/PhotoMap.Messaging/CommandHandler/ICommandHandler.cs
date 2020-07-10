using System;
using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Messaging.CommandHandler
{
    public interface ICommandHandler
    {
        Type CommandType { get; }

        Task HandleAsync(CommandBase command, CancellationToken cancellationToken);
    }
}
