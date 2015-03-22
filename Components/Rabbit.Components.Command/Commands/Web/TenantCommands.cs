using CommandLine;
using Rabbit.Kernel.Environment.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Command.Commands.Web
{
    internal abstract class TenantCommands : Command
    {
        protected readonly IShellSettingsManager ShellSettingsManager;

        protected TenantCommands(IShellSettingsManager shellSettingsManager)
        {
            ShellSettingsManager = shellSettingsManager;
        }
    }

    [Command(CommandAction.Add, "SiteDomain", "添加、设置、删除 指定站点域名。")]
    [CommandAliases("Set-SiteDomain", "Remove-SiteDomain", "Delete-SiteDomain")]
    internal sealed class SetSiteDomain : TenantCommands
    {
        public SetSiteDomain(IShellSettingsManager shellSettingsManager)
            : base(shellSettingsManager)
        {
        }

        [Option('n', "name", Required = true, HelpText = "租户名称，不区分大小写。")]
        public string TenantName { get; set; }

        [OptionList('d', "domain", Required = true, HelpText = "域名列表，格式：domain1.com:domain2.com:domain3.com...")]
        public List<string> Domains { get; set; }

        #region Overrides of Command

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public override void Execute(CommandExecuteContext context)
        {
            var settings = ShellSettingsManager.LoadSettings()
                .FirstOrDefault(i => i.Name.Equals(TenantName, StringComparison.OrdinalIgnoreCase));

            if (settings == null)
            {
                context.WriteLine("找名称为 '{0}' 的租户信息。", TenantName);
                return;
            }
            if (Domains == null)
                Domains = new List<string>();
            Domains = Domains.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()).ToList();

            var host = settings["RequestUrlHost"];
            string[] hosts = null;
            var msg = string.Empty;
            context.Is(() =>
            {
                msg = "添加";
                var list = string.IsNullOrWhiteSpace(host)
                    ? new string[0]
                    : host.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var domains = Domains.Where(i => !list.Any(d => i.Equals(d, StringComparison.OrdinalIgnoreCase)));
                hosts = list.Concat(domains).ToArray();
            }, "Add-SiteDomain");

            context.Is(() =>
            {
                msg = "设置";
                hosts = Domains.ToArray();
            }, "Set-SiteDomain");

            context.Is(() =>
            {
                msg = "删除";
                var list = string.IsNullOrWhiteSpace(host)
                    ? new string[0]
                    : host.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                hosts = list.Where(i => !Domains.Any(d => i.Equals(d, StringComparison.OrdinalIgnoreCase))).ToArray();
            }, "Remove-SiteDomain", "Delete-SiteDomain");

            if (hosts == null)
                return;

            settings["RequestUrlHost"] = string.Join(",", hosts);
            ShellSettingsManager.SaveSettings(settings);

            context.WriteLine(msg + "成功。");
        }

        #endregion Overrides of Command
    }
}