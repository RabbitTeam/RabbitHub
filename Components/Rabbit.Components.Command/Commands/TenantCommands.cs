using CommandLine;
using Rabbit.Components.Command.Utility;
using Rabbit.Kernel.Environment.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.Components.Command.Commands
{
    internal abstract class TenantCommandBase : Command
    {
        protected readonly IShellSettingsManager SettingsManager;

        protected TenantCommandBase(IShellSettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }
    }

    [Command(CommandAction.Set, "Tenant", "添加、更新 一个租户")]
    [CommandAliases("Add-Tenant", "Update-Tenant")]
    internal sealed class SetTenantCommand : TenantCommandBase
    {
        public SetTenantCommand(IShellSettingsManager settingsManager)
            : base(settingsManager)
        {
        }

        [Option('n', "name", Required = true, HelpText = "租户名称，不区分大小写。")]
        public string TenantName { get; set; }

        [Option('s', "state", HelpText = "租户状态，格式：Uninitialized,Running,Disabled,Invalid。", DefaultValue = TenantState.Uninitialized)]
        public TenantState State { get; set; }

        [OptionList('o', "other", HelpText = "其他参数，格式：({DataProvider}={SqlServer}):({DataTablePrefix}={null})")]
        public IList<string> List { get; set; }

        [Option('q', "Quiet", HelpText = "静默执行。", DefaultValue = false)]
        public bool Quiet { get; set; }

        #region Overrides of Command

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public override void Execute(CommandExecuteContext context)
        {
            var settings = SettingsManager.LoadSettings().FirstOrDefault(i => string.Equals(i.Name, TenantName, StringComparison.OrdinalIgnoreCase));
            var isAdd = settings == null;
            if (settings != null && !Quiet)
            {
                context.WriteLine("已经存在名称为 '{0}' 的租户，确定要替换吗？\t输入'Y'继续执行，输入其他则终止执行。", TenantName);
                var t = context.Read("y");
                if (!t.Equals("y", StringComparison.OrdinalIgnoreCase))
                    return;
            }
            else
                settings = new ShellSettings();

            if (List != null)
            {
                foreach (var temp in List.Where(i => !string.IsNullOrWhiteSpace(i)))
                {
                    var index = temp.IndexOf("=", StringComparison.Ordinal);
                    if (index == -1)
                        continue;

                    var key = temp.Substring(0, index).Escape();
                    var value = temp.Substring(index);

                    if (value.StartsWith("="))
                        value = value.Remove(0, 1);

                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    value = value.Escape();
                    settings[key] = value.Escape();
                }
            }

            settings.Name = TenantName;
            settings.State = State;
            SettingsManager.SaveSettings(settings);

            context.WriteLine("{0}租户 '{1}' 成功。", isAdd ? "添加" : "修改", TenantName);
        }

        #endregion Overrides of Command
    }

    [Command(CommandAction.Get, "Tenants", "获取所有租户信息。")]
    internal sealed class GetTenantsCommand : TenantCommandBase
    {
        public GetTenantsCommand(IShellSettingsManager settingsManager)
            : base(settingsManager)
        {
        }

        #region Overrides of Command

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public override void Execute(CommandExecuteContext context)
        {
            var settings = SettingsManager.LoadSettings();
            WriteSetting(settings, context);
        }

        #endregion Overrides of Command

        #region Private Method

        public static void WriteSetting(IEnumerable<ShellSettings> settings, CommandContext context)
        {
            context.WriteLine("名称\t\t\t状态\t\t\t其他");
            foreach (var setting in settings.Where(i => i != null))
            {
                var builder = new StringBuilder();
                var ig = new[] { "name", "state" };
                foreach (var key in setting.Keys.Where(i => !ig.Any(g => g.Equals(i, StringComparison.OrdinalIgnoreCase))))
                {
                    builder.AppendFormat("{0}={1}", key, setting[key]);
                }
                context.WriteLine(string.Format("{0}\t\t\t{1}\t\t\t{2}", setting.Name, setting.State, builder));
            }
        }

        #endregion Private Method
    }

    [Command(CommandAction.Get, "Tenant", "获取一个租户信息。")]
    internal sealed class GetTenantCommand : TenantCommandBase
    {
        public GetTenantCommand(IShellSettingsManager settingsManager)
            : base(settingsManager)
        {
        }

        #region Overrides of Command

        [Option('n', "name", HelpText = "租户名称。", Required = true)]
        public string TenantName { get; set; }

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public override void Execute(CommandExecuteContext context)
        {
            var setting = SettingsManager.LoadSettings()
                .FirstOrDefault(i => TenantName.Equals(i.Name, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
                context.WriteLine("找不到任何可用的租户信息。");

            GetTenantsCommand.WriteSetting(new[] { setting }, context);
        }

        #endregion Overrides of Command
    }

    [Command(CommandAction.Delete, "Tenant", "删除一个租户信息。")]
    [CommandAliases("Remove-Tenant")]
    internal sealed class DeleteTenantCommand : TenantCommandBase
    {
        public DeleteTenantCommand(IShellSettingsManager settingsManager)
            : base(settingsManager)
        {
        }

        [Option('n', "name", HelpText = "租户名称。", Required = true)]
        public string TenantName { get; set; }

        [Option('q', "Quiet", HelpText = "静默执行。", DefaultValue = false)]
        public bool Quiet { get; set; }

        #region Overrides of Command

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public override void Execute(CommandExecuteContext context)
        {
            var setting = SettingsManager.LoadSettings()
                .FirstOrDefault(i => i.Name.Equals(TenantName, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                context.WriteLine("找不到名称为 '{0}' 的租户。", TenantName);
                return;
            }
            if (!Quiet)
            {
                context.WriteLine("确定要删除租户 '{0}' 吗?\t输入'Y'继续执行，输入其他则终止执行。", setting.Name);
                var t = context.Read("y");
                if (!t.Equals("y", StringComparison.OrdinalIgnoreCase))
                    return;
            }

            SettingsManager.DeleteSettings(TenantName);
            context.WriteLine("成功删除租户 '{0}'。", TenantName);
        }

        #endregion Overrides of Command
    }
}