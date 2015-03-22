using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Utility.Extensions;

namespace Rabbit.Kernel.Extensions
{
    /// <summary>
    /// 扩展描述符条目过滤器上下文。
    /// </summary>
    public class ExtensionDescriptorEntryFilterContext
    {
        /// <summary>
        /// 初始化一个新的扩展描述符条目过滤器上下文。
        /// </summary>
        /// <param name="entry">扩展描述符条目。</param>
        public ExtensionDescriptorEntryFilterContext(ExtensionDescriptorEntry entry)
        {
            entry.NotNull("entry");
            Entry = entry;
            Valid = true;
        }

        /// <summary>
        /// 扩展描述符条目。
        /// </summary>
        public ExtensionDescriptorEntry Entry { get; private set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        public bool Valid { get; set; }
    }

    /// <summary>
    /// 一个抽象的扩展描述符过滤器。
    /// </summary>
    public interface IExtensionDescriptorFilter : IDependency
    {
        /// <summary>
        /// 在发现扩展时执行。
        /// </summary>
        /// <param name="context">扩展描述符条目过滤器上下文。</param>
        void OnDiscovery(ExtensionDescriptorEntryFilterContext context);
    }
}