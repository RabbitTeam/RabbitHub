using Autofac;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.ShellBuilders.Models;

namespace Rabbit.Kernel.Environment.ShellBuilders
{
    /// <summary>
    /// 一个抽象的外壳容器工厂。
    /// </summary>
    public interface IShellContainerFactory
    {
        /// <summary>
        /// 创建一个外壳容器。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <param name="blueprint">外壳蓝图。</param>
        /// <returns>外壳容器。</returns>
        ILifetimeScope CreateContainer(ShellSettings settings, ShellBlueprint blueprint);
    }
}