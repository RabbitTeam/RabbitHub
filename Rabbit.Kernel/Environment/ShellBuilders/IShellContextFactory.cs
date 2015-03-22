using Rabbit.Kernel.Environment.Configuration;

namespace Rabbit.Kernel.Environment.ShellBuilders
{
    /// <summary>
    /// 一个抽象的外壳上下文工厂。
    /// </summary>
    public interface IShellContextFactory
    {
        /// <summary>
        /// 创建一个外壳上下文工厂。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <returns>外壳上下文。</returns>
        ShellContext CreateShellContext(ShellSettings settings);
    }
}