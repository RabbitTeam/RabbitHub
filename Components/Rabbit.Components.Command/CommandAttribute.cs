using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Linq;

namespace Rabbit.Components.Command
{
    /// <summary>
    /// 命令标记。
    /// </summary>
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个新的命令标记。
        /// </summary>
        /// <param name="commandName">命令名称。</param>
        public CommandAttribute(string commandName)
        {
            CommandName = commandName.NotEmptyOrWhiteSpace("commandName");
        }

        /// <summary>
        /// 初始化一个新的命令标记。
        /// </summary>
        /// <param name="commandAction">命令动作。</param>
        /// <param name="commandName">命令名称。</param>
        public CommandAttribute(CommandAction commandAction, string commandName)
        {
            Action = commandAction;
            CommandName = commandName.NotEmptyOrWhiteSpace("commandName");
        }

        /// <summary>
        /// 初始化一个新的命令标记。
        /// </summary>
        /// <param name="commandAction">命令动作。</param>
        /// <param name="commandName">命令名称。</param>
        /// <param name="description">命令说明。</param>
        public CommandAttribute(CommandAction commandAction, string commandName, string description)
            : this(commandName)
        {
            Action = commandAction;
            Description = description.NotEmptyOrWhiteSpace("description");
        }

        /// <summary>
        /// 命令说明。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 命令的调用名称。
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// 动作。
        /// </summary>
        public CommandAction Action { get; set; }
    }

    /// <summary>
    /// 命令别名标记。
    /// </summary>
    public sealed class CommandAliasesAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个新的命令别名。
        /// </summary>
        /// <param name="aliases">命令别名。</param>
        public CommandAliasesAttribute(params string[] aliases)
        {
            Aliases = aliases.NotNull("aliases");
            var count = aliases.Count();
            if (count > aliases.Distinct().Count())
                throw new Exception("不允许有重复的别名。");
        }

        /// <summary>
        /// 别名名称。
        /// </summary>
        public string[] Aliases { get; private set; }
    }
}