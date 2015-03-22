using Rabbit.Kernel.Extensions.Models;
using System.Collections.Generic;

namespace Rabbit.Kernel.Extensions.Folders
{
    /// <summary>
    /// 一个抽象的扩展文件夹。
    /// </summary>
    public interface IExtensionFolders
    {
        /// <summary>
        /// 可用的扩展。
        /// </summary>
        /// <returns>扩展描述条目符集合。</returns>
        IEnumerable<ExtensionDescriptorEntry> AvailableExtensions();
    }
}