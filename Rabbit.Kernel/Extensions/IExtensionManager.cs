using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Utility.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Extensions
{
    /// <summary>
    /// 一个抽象的扩展管理者。
    /// </summary>
    public interface IExtensionManager
    {
        /// <summary>
        /// 可用的扩展。
        /// </summary>
        /// <returns>扩展描述符条目集合。</returns>
        IEnumerable<ExtensionDescriptorEntry> AvailableExtensions();

        /// <summary>
        /// 可用的特性。
        /// </summary>
        /// <returns>特性描述符集合。</returns>
        IEnumerable<FeatureDescriptor> AvailableFeatures();

        /// <summary>
        /// 根据扩展Id获取指定的扩展描述符条目。
        /// </summary>
        /// <param name="id">扩展Id。</param>
        /// <returns>扩展描述符。</returns>
        ExtensionDescriptorEntry GetExtension(string id);

        /// <summary>
        /// 加载特性。
        /// </summary>
        /// <param name="featureDescriptors">特性描述符。</param>
        /// <returns>特性集合。</returns>
        IEnumerable<Feature> LoadFeatures(IEnumerable<FeatureDescriptor> featureDescriptors);
    }

    /// <summary>
    /// 扩展管理者扩展方法。
    /// </summary>
    public static class ExtensionManagerExtensions
    {
        /// <summary>
        /// 获取开启的特性。
        /// </summary>
        /// <param name="extensionManager">扩展管理者。</param>
        /// <param name="descriptor">外壳描述符。</param>
        /// <returns></returns>
        public static IEnumerable<FeatureDescriptor> EnabledFeatures(this IExtensionManager extensionManager, ShellDescriptor descriptor)
        {
            extensionManager.NotNull("extensionManager");

            var features = extensionManager.AvailableFeatures();
            if (descriptor != null)
                features = features.Where(fd => descriptor.Features.Any(sf => sf.Name == fd.Id));

            return features.ToArray();
        }
    }
}