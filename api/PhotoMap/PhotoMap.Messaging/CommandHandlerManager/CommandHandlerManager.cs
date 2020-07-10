using System;
using System.Collections.Generic;
using System.Linq;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Messaging.CommandHandlerManager
{
    public class CommandHandlerManager : ICommandHandlerManager
    {
        private readonly Dictionary<Type, ICommandHandler> _commandHandlerMap;

        public CommandHandlerManager(IEnumerable<ICommandHandler> commandHandlers)
        {
            _commandHandlerMap = commandHandlers.ToDictionary(a => a.CommandType, b => b);
        }

        public ICommandHandler GetHandler(CommandBase commandBase)
        {
            var commandType = commandBase.GetType();

            return _commandHandlerMap.TryGetValue(commandType, out var commandHandler) ? commandHandler : null;
        }
    }
}
