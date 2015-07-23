using Rabbit.Kernel.Caching;
using Rabbit.Kernel.FileSystems.VirtualPath;

namespace Rabbit.Kernel.FileSystems
{
    /// <summary>
    /// 一个抽象的文件夹。
    /// </summary>
    public interface IFolder : IVirtualPathProviderBase, IVolatileProvider
    {
        /// <summary>
        /// 创建文件。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <param name="content">文件内容。</param>
        void CreateFile(string path, string content);

        /// <summary>
        /// 读取文件内容。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <returns>文件内容。</returns>
        string ReadFile(string path);

        /// <summary>
        /// 储存文件。
        /// </summary>
        /// <param name="sourceFileName">源文件名称。</param>
        /// <param name="destinationPath">目标文件路径。</param>
        void StoreFile(string sourceFileName, string destinationPath);

        /// <summary>
        /// 指定路径的文件或文件夹是否发生变更。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>挥发令牌。</returns>
        IVolatileToken WhenPathChanges(string path);

        /// <summary>
        /// 获取路径的虚拟路径。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>虚拟路径。</returns>
        string GetVirtualPath(string path);
    }
}