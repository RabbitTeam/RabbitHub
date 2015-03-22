using Autofac;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Environment.ShellBuilders.Models;

namespace Rabbit.Kernel.Environment.ShellBuilders
{
    /// <summary>
    /// 外壳上下文。
    /// </summary>
    public sealed class ShellContext
    {
        /// <summary>
        /// 外壳设置。
        /// </summary>
        public ShellSettings Settings { get; set; }

        /// <summary>
        /// 外壳描述符。
        /// </summary>
        public ShellDescriptor Descriptor { get; set; }

        /// <summary>
        /// 外壳蓝图。
        /// </summary>
        public ShellBlueprint Blueprint { get; set; }

        /// <summary>
        /// 外壳容器。
        /// </summary>
        public ILifetimeScope Container { get; set; }

        /// <summary>
        /// 外壳。
        /// </summary>
        public IShell Shell { get; set; }
    }
}