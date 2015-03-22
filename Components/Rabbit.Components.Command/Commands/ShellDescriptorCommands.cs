/*using Autofac;
using CommandLine;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.Descriptor;
using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Command.Commands
{
    internal abstract class ShellDescriptorCommandBase : Command
    {
        private readonly IHost _host;

        protected ShellDescriptorCommandBase(IHost host)
        {
            _host = host;
        }

        protected IShellDescriptorManager GetShellDescriptorManager(string tenantName)
        {
            var context = _host.GetShellContext(new ShellSettings { Name = tenantName });
            if (context == null || context.Container == null)
                return null;

            return context.Container.Resolve<IShellDescriptorManager>();
        }
    }

    [Command(CommandAction.Update, "ShellDescriptor", Description = "更新、添加、删除 外壳描述符。")]
    [CommandAliases("Add-ShellDescriptor", "Remove-ShellDescriptor", "Delete-ShellDescriptor")]
    internal sealed class ShellDescriptorCommands : ShellDescriptorCommandBase
    {
        private readonly IShellDescriptorCache _shellDescriptorCache;

        public ShellDescriptorCommands(IHost host, IShellDescriptorCache shellDescriptorCache)
            : base(host)
        {
            _shellDescriptorCache = shellDescriptorCache;
        }

        [Option('n', "name", Required = true, HelpText = "对应的租户名称。")]
        public string TenantName { get; set; }

        [OptionList('f', "features", HelpText = "开启的功能名称，格式：Rabbit.Component1:Rabbit.Component2...")]
        public List<string> Features { get; set; }

        #region Overrides of Command

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public override void Execute(CommandExecuteContext context)
        {
            Features = Features ?? new List<string>();
            var manager = GetShellDescriptorManager(TenantName);
            if (manager == null)
            {
                context.WriteLine("租户 '{0}' 不存在。", TenantName);
                return;
            }

            var features =
                Features.Distinct().Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.CamelFriendly()).Distinct().Select(i => new ShellFeature { Name = i }).ToArray();

            var descriptor = manager.GetShellDescriptor();

            var msg = string.Empty;

            context.Is(() =>
            {
                if (!Features.Any())
                {
                    msg = "请输入需要添加的功能信息。";
                    return;
                }
                manager.UpdateShellDescriptor(features);
                descriptor.Features = features;
                _shellDescriptorCache.Store(TenantName, descriptor);
                msg = "更新成功。";
            }, "Update-ShellDescriptor");

            context.Is(() =>
            {
                if (!Features.Any())
                {
                    msg = "请输入需要添加的功能信息。";
                    return;
                }
                var outFeatures = descriptor.Features.ToArray();
                var comparer = new ShellFeatureEqualityComparer();
                features = outFeatures.Concat(features.Where(i => !outFeatures.Any(f => comparer.Equals(i, f)))).ToArray();
                manager.UpdateShellDescriptor(features);
                descriptor.Features = features;
                _shellDescriptorCache.Store(TenantName, descriptor);
                msg = "添加成功。";
            }, "Add-ShellDescriptor");

            context.Is(() =>
            {
                if (!Features.Any())
                {
                    manager.DeleteShellDescriptor();
                }
                else
                {
                    var outFeatures = descriptor.Features.ToArray();
                    var comparer = new ShellFeatureEqualityComparer();
                    features = outFeatures.Where(i => !features.Any(f => comparer.Equals(i, f))).ToArray();
                    manager.UpdateShellDescriptor(features);
                    descriptor.Features = features;
                    _shellDescriptorCache.Store(TenantName, descriptor);
                }
                msg = "删除成功。";
            }, "Remove-ShellDescriptor", "Delete-ShellDescriptor");

            context.WriteLine(msg);
        }

        #endregion Overrides of Command

        #region Help Class

        private class ShellFeatureEqualityComparer : IEqualityComparer<ShellFeature>
        {
            #region Implementation of IEqualityComparer<in ShellFeature>

            /// <summary>
            /// 确定指定的对象是否相等。
            /// </summary>
            /// <returns>
            /// 如果指定的对象相等，则为 true；否则为 false。
            /// </returns>
            /// <param name="x">要比较的第一个类型为 <see name="T"/> 的对象。</param><param name="y">要比较的第二个类型为 <see name="T"/> 的对象。</param>
            public bool Equals(ShellFeature x, ShellFeature y)
            {
                return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// 返回指定对象的哈希代码。
            /// </summary>
            /// <returns>
            /// 指定对象的哈希代码。
            /// </returns>
            /// <param name="obj"><see cref="T:System.Object"/>，将为其返回哈希代码。</param><exception cref="T:System.ArgumentNullException"><paramref name="obj"/> 的类型为引用类型，<paramref name="obj"/> 为 null。</exception>
            public int GetHashCode(ShellFeature obj)
            {
                return obj.Name.GetHashCode();
            }

            #endregion Implementation of IEqualityComparer<in ShellFeature>
        }

        #endregion Help Class
    }

    [Command(CommandAction.Get, "ShellDescriptor", Description = "获取外壳描述符信息。")]
    internal sealed class GetShellDescriptorCommands : ShellDescriptorCommandBase
    {
        public GetShellDescriptorCommands(IHost host)
            : base(host)
        {
        }

        [Option('n', "name", Required = true, HelpText = "对应的租户名称。")]
        public string TenantName { get; set; }

        #region Overrides of Command

        /// <summary>
        /// 执行命令。
        /// </summary>
        /// <param name="context">命令执行上下文。</param>
        public override void Execute(CommandExecuteContext context)
        {
            var manager = GetShellDescriptorManager(TenantName);
            if (manager == null)
            {
                context.WriteLine("找不到租户 '{0}'。", TenantName);
                return;
            }
            var features = manager.GetShellDescriptor().Features ?? new ShellFeature[0];
            var shellFeatures = features as ShellFeature[] ?? features.ToArray();
            context.WriteLine(!shellFeatures.Any() ? "租户 '{0}' 没有任何特性。" : string.Format("{0}{1}", "包含的特性：" + Environment.NewLine, string.Join(Environment.NewLine, shellFeatures.Select(i => i.Name))));
        }

        #endregion Overrides of Command
    }
}*/