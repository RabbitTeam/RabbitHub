using Rabbit.Kernel.Caching;

namespace Rabbit.Kernel.FileSystems.VirtualPath
{
    /// <summary>
    /// 一个抽象的虚拟路径监视者。
    /// </summary>
    public interface IVirtualPathMonitor : IVolatileProvider
    {
        /// <summary>
        /// 检测路径是否被更改。
        /// </summary>
        /// <param name="virtualPath">需要监测的虚拟路径。</param>
        /// <returns>挥发令牌。</returns>
        IVolatileToken WhenPathChanges(string virtualPath);
    }
}