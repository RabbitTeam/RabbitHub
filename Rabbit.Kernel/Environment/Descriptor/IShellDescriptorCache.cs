using Rabbit.Kernel.Environment.Descriptor.Models;

namespace Rabbit.Kernel.Environment.Descriptor
{
    /// <summary>
    /// 一个抽象的外壳描述符缓存。
    /// </summary>
    public interface IShellDescriptorCache
    {
        /// <summary>
        /// 从缓存中抓取外壳描述符。
        /// </summary>
        /// <param name="shellName">外壳名称。</param>
        /// <returns>外壳描述符。</returns>
        ShellDescriptor Fetch(string shellName);

        /// <summary>
        /// 将一个外壳描述符存储到缓存中。
        /// </summary>
        /// <param name="shellName">外壳名称。</param>
        /// <param name="descriptor">外壳描述符。</param>
        void Store(string shellName, ShellDescriptor descriptor);
    }
}