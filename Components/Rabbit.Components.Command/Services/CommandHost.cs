using CommandLine;
using CommandLine.Text;
using Rabbit.Components.Command.Utility;
using Rabbit.Kernel;
using System;
using System.Linq;

namespace Rabbit.Components.Command.Services
{
    /// <summary>
    /// 一个抽象的命令主机。
    /// </summary>
    public interface ICommandHost : IDependency
    {
        /// <summary>
        /// 执行。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <returns>执行成功则返回true，否则返回false。</returns>
        bool Execute(CommandContext context);
    }

    internal sealed class CommandHost : ICommandHost
    {
        private readonly ICommandService _commandService;

        #region Constructor

        public CommandHost(ICommandService commandService)
        {
            _commandService = commandService;
        }

        #endregion Constructor

        #region Implementation of ICommandHost

        /// <summary>
        /// 执行。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <returns>执行成功则返回true，否则返回false。</returns>
        public bool Execute(CommandContext context)
        {
            var args = context.Args ?? new string[0];
            //去除开头的空字符串。
            args = args.SkipWhile(string.IsNullOrWhiteSpace).ToArray();

            //不存在任何参数。
            if (!args.Any())
            {
                //否则显示帮助信息。
                ShowDefaultHelp(context, null);
                return false;
            }

            //得到命令名称。
            var commandName = args.First();
            //得到精简后的解析参数。
            args = args.Skip(1).ToArray();

            ICommand command;

            //如果成功获取命令对象则执行。
            if (GetCommand(commandName, args, out command))
                return Executed(command, GetCommandExecuteContext(context, args, commandName));

            if (command != null)
            {
                ShowCommandHelp(context, command);
            }
            else
            {
                //否则显示帮助信息。
                ShowDefaultHelp(context, commandName);
            }
            return false;
        }

        #endregion Implementation of ICommandHost

        #region Private Method

        /// <summary>
        /// 获取命令执行上下文。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        /// <param name="args">参数。</param>
        /// <param name="commandName">命令名称。</param>
        /// <returns>命令执行上下文。</returns>
        private static CommandExecuteContext GetCommandExecuteContext(CommandContext context, string[] args, string commandName)
        {
            return new CommandExecuteContext
            {
                Args = args,
                Reader = context.Reader,
                Writer = context.Writer,
                FullArgs = context.Args,
                CommandName = commandName
            };
        }

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="command">命令对象。</param>
        /// <param name="context">命令上下文。</param>
        /// <returns>是否执行成功。</returns>
        private static bool Executed(ICommand command, CommandExecuteContext context)
        {
            try
            {
                command.Is(c => c.Executing(context));
                command.Execute(context);
                return true;
            }
            catch (Exception exception)
            {
                if (command.Is(c => c.Fail(context, exception)))
                    throw;
            }
            finally
            {
                command.Is(c => c.Executed(context));
            }
            return false;
        }

        /// <summary>
        /// 获取命令。
        /// </summary>
        /// <param name="commandName">命令名称。</param>
        /// <param name="args">转换参数。</param>
        /// <param name="command">命令实例。</param>
        /// <returns>是否获取成功。</returns>
        private bool GetCommand(string commandName, string[] args, out ICommand command)
        {
            command = _commandService.GetCommand(commandName);
            return command != null && Parser.Default.ParseArguments(args, command);
        }

        /// <summary>
        /// 显示命令帮助。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <param name="command">命令实例。</param>
        private static void ShowCommandHelp(CommandContext context, ICommand command)
        {
            //基本信息
            var help = CommandHelper.GetHelpText();

            help.AddPreOptionsLine(string.Format("语句 \"{0}\" 存在错误，请参考下面的帮助信息。", string.Join(" ", context.Args)));
            help.AddOptions(command);

            context.Writer.Write(help);
        }

        /// <summary>
        /// 显示默认帮助。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <param name="commandName">命令名称。</param>
        /// <returns>是否执行成功。</returns>
        private void ShowDefaultHelp(CommandContext context, string commandName)
        {
            var help = CommandHelper.GetHelpText();

            //基本信息
            help.Heading = new HeadingInfo("Rabbit CommandLine", GetType().Assembly.GetName().Version.ToString());
            help.Copyright = new CopyrightInfo("Rabbit", DateTime.Now.Year);

            help.AddPreOptionsLine(Environment.NewLine);

            help.AddPreOptionsLine("转义规则：" + Environment.NewLine);
            foreach (var text in CommandHelper.EscapeDictionary.Select(i => string.Format("'{0}'    >    '{1}'", i.Key, i.Value)))
            {
                help.AddPreOptionsLine(text);
            }

            help.AddPreOptionsLine(Environment.NewLine);

            var message = "输入 \"Get-Commands\" 获取所有命令。";
            if (commandName != null)
            {
                message = string.Format("找不到名称为 {0} 的命令，", commandName) + message;
            }
            help.AddPreOptionsLine(message);
            context.Writer.WriteLine(help);
        }

        #endregion Private Method
    }
}