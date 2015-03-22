namespace Rabbit.Kernel.Extensions
{
    /// <summary>
    /// 一个抽象的扩展装载机协调员。
    /// </summary>
    public interface IExtensionLoaderCoordinator
    {
        /// <summary>
        /// 安装扩展。
        /// </summary>
        void SetupExtensions();
    }
}