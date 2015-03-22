using Rabbit.Kernel;
using System;
using System.Linq;

namespace Rabbit.Components.Command.Services
{
    internal interface ICommandService : ISingletonDependency
    {
        ICommand GetCommand(string commandName);

        ICommand[] GetCommands();
    }

    internal sealed class CommandService : ICommandService
    {
        private readonly Lazy<ICommandHarvester> _commandHarvester;

        public CommandService(Lazy<ICommandHarvester> commandHarvester)
        {
            _commandHarvester = commandHarvester;
        }

        #region Implementation of ICommandService

        public ICommand GetCommand(string commandName)
        {
            var commands = GetCommands();

            return commands.SingleOrDefault(i =>
            {
                var list = new[] { i.CommandName };

                if (i.CommandAliases != null)
                    list = list.Concat(i.CommandAliases).ToArray();

                return list.Any(name => string.Equals(name, commandName, StringComparison.OrdinalIgnoreCase));
            });
        }

        public ICommand[] GetCommands()
        {
            return _commandHarvester.Value.GetCommands().ToArray();
        }

        #endregion Implementation of ICommandService
    }
}