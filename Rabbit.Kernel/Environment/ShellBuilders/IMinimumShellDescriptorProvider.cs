using Rabbit.Kernel.Environment.Descriptor.Models;
using System.Collections.Generic;

namespace Rabbit.Kernel.Environment.ShellBuilders
{
    /// <summary>
    /// 一个抽象的最新外壳描述符提供者。
    /// </summary>
    public interface IMinimumShellDescriptorProvider
    {
        /// <summary>
        /// 获取外壳描述符。
        /// </summary>
        /// <param name="features">外壳特性描述符。</param>
        void GetFeatures(ICollection<ShellFeature> features);
    }
}