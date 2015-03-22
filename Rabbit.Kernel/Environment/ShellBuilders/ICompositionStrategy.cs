using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Environment.ShellBuilders.Models;

namespace Rabbit.Kernel.Environment.ShellBuilders
{
    /// <summary>
    /// 一个抽象的组合策略。
    /// </summary>
    public interface ICompositionStrategy
    {
        /// <summary>
        /// 组合外壳蓝图。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <param name="descriptor">外壳描述符。</param>
        /// <returns>外壳蓝图。</returns>
        ShellBlueprint Compose(ShellSettings settings, ShellDescriptor descriptor);
    }
}