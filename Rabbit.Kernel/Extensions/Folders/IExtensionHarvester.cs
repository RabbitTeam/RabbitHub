using Rabbit.Kernel.Extensions.Models;
using System.Collections.Generic;

namespace Rabbit.Kernel.Extensions.Folders
{
    /// <summary>
    /// 一个抽象的扩展收集者。
    /// </summary>
    public interface IExtensionHarvester
    {
        /// <summary>
        /// 收集扩展。
        /// </summary>
        /// <param name="paths">需要进行收集的路径。</param>
        /// <param name="extensionType">扩展类型。</param>
        /// <param name="manifestName">清单文件名称。</param>
        /// <param name="manifestIsOptional">清单文件是否是可选的。</param>
        /// <returns>扩展描述符集合。</returns>
        IEnumerable<ExtensionDescriptorEntry> HarvestExtensions(IEnumerable<string> paths, string extensionType, string manifestName, bool manifestIsOptional);
    }
}