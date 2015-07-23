using Rabbit.Kernel.Caching;

namespace Rabbit.Kernel.FileSystems.VirtualPath
{
    /// <summary>
    /// 一个抽象的虚拟路径提供者。
    /// </summary>
    public interface IVirtualPathProvider : IVirtualPathProviderBase, IVolatileProvider
    {
    }
}