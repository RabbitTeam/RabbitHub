using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Works;

namespace Rabbit.Kernel.Environment
{
    /// <summary>
    /// 一个抽象的主机。
    /// </summary>
    public interface IHost
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        void Initialize();

        /// <summary>
        /// 重新加载扩展。
        /// </summary>
        void ReloadExtensions();

        /// <summary>
        /// 获取一个外壳上下文。
        /// </summary>
        /// <param name="shellSettings">外壳设置。</param>
        /// <returns>外壳上下文。</returns>
        ShellContext GetShellContext(ShellSettings shellSettings);

        /// <summary>
        /// 创建一个独立的环境。
        /// </summary>
        /// <param name="shellSettings">外壳设置。</param>
        /// <returns>工作上下文范围。</returns>
        IWorkContextScope CreateStandaloneEnvironment(ShellSettings shellSettings);
    }
}