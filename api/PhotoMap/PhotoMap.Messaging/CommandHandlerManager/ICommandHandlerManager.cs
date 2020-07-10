using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Messaging.CommandHandlerManager
{
    public interface ICommandHandlerManager
    {
        ICommandHandler GetHandler(CommandBase commandBase);
    }
}
