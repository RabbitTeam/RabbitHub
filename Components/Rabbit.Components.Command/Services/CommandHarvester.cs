using Rabbit.Kernel;
using Rabbit.Kernel.Caching;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Command.Services
{
    internal interface ICommandHarvester : ISingletonDependency
    {
        IEnumerable<ICommand> GetCommands();
    }

    /// <summary>
    /// 一个抽象的命令提供程序。
    /// </summary>
    public interface ICommandProvider : ISingletonDependency
    {
        /// <summary>
        /// 获取命令。
        /// </summary>
        /// <param name="commands">命令集合。</param>
        void GetCommands(ICollection<ICommand> commands);
    }

    internal sealed class CommandHarvester : ICommandHarvester
    {
        #region Field

        private readonly IEnumerable<ICommandProvider> _commandProviders;
        private readonly ICacheManager _cacheManager;

        #endregion Field

        #region Constructor

        public CommandHarvester(IEnumerable<ICommandProvider> commandProviders, ICacheManager cacheManager)
        {
            _commandProviders = commandProviders;
            _cacheManager = cacheManager;
        }

        #endregion Constructor

        #region Implementation of ICommandHarvester

        public IEnumerable<ICommand> GetCommands()
        {
            return _cacheManager.Get("Commands", ctx =>
            {
                var commands = new List<ICommand>();

                foreach (var commandProvider in _commandProviders)
                    commandProvider.GetCommands(commands);

                return commands.Where(c => c != null).Distinct().ToArray();
            });
        }

        #endregion Implementation of ICommandHarvester
    }

    internal sealed class DefaultCommandProvider : ICommandProvider
    {
        #region Field

        private readonly IEnumerable<ICommand> _commands;

        #endregion Field

        #region Constructor

        public DefaultCommandProvider(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        #endregion Constructor

        #region Implementation of ICommandProvider

        /// <summary>
        /// 获取命令。
        /// </summary>
        /// <param name="commands">命令集合。</param>
        public void GetCommands(ICollection<ICommand> commands)
        {
            foreach (var command in _commands)
            {
                if (command is Command)
                {
                    var type = command.GetType();
                    var commandAttribute = type.GetCustomAttributes(typeof(CommandAttribute), false).OfType<CommandAttribute>().FirstOrDefault();
                    var commandAliases = type.GetCustomAttributes(typeof(CommandAliasesAttribute), false).OfType<CommandAliasesAttribute>().FirstOrDefault();
                    if (commandAttribute != null)
                    {
                        var c = command as Command;
                        c.Action = commandAttribute.Action;
                        c.Name = commandAttribute.CommandName;
                        c.Description = commandAttribute.Description;
                    }
                    if (commandAliases != null)
                    {
                        var c = command as Command;
                        c.CommandAliases = commandAliases.Aliases;
                    }
                }
                commands.Add(command);
            }
        }

        #endregion Implementation of ICommandProvider
    }
}