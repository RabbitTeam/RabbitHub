using CommandLine;
using Rabbit.Components.Command.Services;
using Rabbit.Components.Command.Utility;
using System;
using System.Linq;

namespace Rabbit.Components.Command.Commands
{
    [Command(CommandAction.Get, "Commands", "获取所有的命令信息。")]
    internal sealed class GetCommands : Command
    {
        private readonly ICommandService _commandService;

        public GetCommands(ICommandService commandService)
        {
            _commandService = commandService;
        }

        #region Overrides of Command

        /// <summary>
        ///     执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public override void Execute(CommandExecuteContext context)
        {
            var commands = _commandService.GetCommands();

            //命令信息
            if (commands == null)
                return;

            var help = CommandHelper.GetHelpText();
            foreach (var text in commands
                .Select(i =>
                {
                    var usage = i.GetUsage();
                    return string.IsNullOrWhiteSpace(usage) ? i.CommandName : usage;
                }))
            {
                help.AddPreOptionsLine(text);
            }

            context.WriteLine(help);
        }

        #endregion Overrides of Command
    }

    [Command(CommandAction.Get, "Command", "获取所有的命令信息。")]
    internal sealed class GetCommand : Command
    {
        private readonly ICommandService _commandService;

        public GetCommand(ICommandService commandService)
        {
            _commandService = commandService;
        }

        [Option('n', "name", Required = true, HelpText = "命令的名称。")]
        public string FindName { get; set; }

        #region Overrides of Command

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public override void Execute(CommandExecuteContext context)
        {
            var command = _commandService.GetCommand(FindName);

            context.Write(Environment.NewLine);
            if (command == null)
            {
                context.WriteLine("找不到名称为 \"{0}\"，的命令。", FindName);
            }
            else
            {
                context.WriteLine("下面是命令 '{0}' 的帮助信息。", FindName);
                context.WriteLine(command.GetUsage());
            }
        }

        #endregion Overrides of Command
    }
}