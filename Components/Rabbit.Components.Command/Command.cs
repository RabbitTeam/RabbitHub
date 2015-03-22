using Rabbit.Components.Command.Utility;
using Rabbit.Kernel;
using System;
using System.Collections.Generic;

namespace Rabbit.Components.Command
{
    /// <summary>
    /// 一个抽象的命令。
    /// </summary>
    public interface ICommand : IDependency
    {
        /// <summary>
        /// 命令的调用名称。
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// 命令别名。
        /// </summary>
        string[] CommandAliases { get; }

        /// <summary>
        /// 命令说明。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        void Execute(CommandExecuteContext context);

        /// <summary>
        /// 获取使用方法。
        /// </summary>
        /// <returns>使用方法字符串。</returns>
        string GetUsage();
    }

    /// <summary>
    /// 命令抽象类。
    /// </summary>
    public abstract class Command : ICommand
    {
        #region Field

        private string _name;
        private string _description;

        #endregion Field

        #region Implementation of ICommandProvider

        /// <summary>
        /// 命令的调用名称。
        /// </summary>
        public string CommandName
        {
            get
            {
                return CommandHelper.GetCommandName(Action, Name);
            }
        }

        /// <summary>
        /// 命令别名。
        /// </summary>
        public virtual string[] CommandAliases { get; internal set; }

        /// <summary>
        /// 命令说明。
        /// </summary>
        public virtual string Description
        {
            get
            {
                return _description ?? string.Empty;
            }
            internal set { _description = value; }
        }

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public abstract void Execute(CommandExecuteContext context);

        /// <summary>
        /// 获取使用方法。
        /// </summary>
        /// <returns>使用方法字符串。</returns>
        public virtual string GetUsage()
        {
            var help = CommandHelper.GetHelpText();

            help.AddDashesToOption = true;
            //            help.AdditionalNewLineAfterOption = true;
            var commandNames = new List<string> { CommandName };
            if (CommandAliases != null)
                commandNames.AddRange(CommandAliases);

            help.AddPreOptionsLine(string.Format("{0}\t\t{1}", string.Join(", ", commandNames), Description));
            help.AddOptions(this);
            return help;
        }

        #endregion Implementation of ICommandProvider

        #region Virtual Method

        /// <summary>
        /// 准备执行命令之前时执行。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        protected internal virtual void Executing(CommandExecuteContext context)
        {
        }

        /// <summary>
        /// 命令执行完成之后执行。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        public virtual void Executed(CommandExecuteContext context)
        {
        }

        /// <summary>
        /// 执行命令出错时执行。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <param name="exception">异常信息。</param>
        /// <returns>是否抛出异常，如果需要抛出异常则返回true，否则返回false。</returns>
        protected internal virtual bool Fail(CommandExecuteContext context, Exception exception)
        {
            context.WriteLine("执行语句 {0} 时发生了错误，详细信息：{1}", string.Join(" ", context.Args), exception);
            return false;
        }

        /// <summary>
        /// 命令的动作。
        /// </summary>
        protected internal virtual CommandAction Action { get; internal set; }

        /// <summary>
        /// 命令的调用名称。
        /// </summary>
        public virtual string Name
        {
            get
            {
                return string.IsNullOrWhiteSpace(_name) ? string.Empty : _name;
            }
            internal set
            {
                _name = value;
            }
        }

        #endregion Virtual Method
    }

    /// <summary>
    /// 命令扩展方法。
    /// </summary>
    internal static class CommandExtensions
    {
        public static void Is(this ICommand command, Action<Command> action)
        {
            if (!(command is Command) || action == null)
                return;
            action(command as Command);
        }

        public static T Is<T>(this ICommand command, Func<Command, T> func)
        {
            if (!(command is Command) || func == null)
                return default(T);
            return func(command as Command);
        }
    }
}