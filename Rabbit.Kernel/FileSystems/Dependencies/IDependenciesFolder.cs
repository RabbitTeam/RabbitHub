using Rabbit.Kernel.Caching;
using System.Collections.Generic;

namespace Rabbit.Kernel.FileSystems.Dependencies
{
    /// <summary>
    /// 一个抽象的依赖项文件夹。
    /// </summary>
    public interface IDependenciesFolder : IVolatileProvider
    {
        /// <summary>
        /// 获取一个依赖项描述符。
        /// </summary>
        /// <param name="moduleName">模块名称。</param>
        /// <returns>依赖项描述符。</returns>
        DependencyDescriptor GetDescriptor(string moduleName);

        /// <summary>
        /// 装载所有依赖项描述符。
        /// </summary>
        /// <returns>依赖项描述符集合。</returns>
        IEnumerable<DependencyDescriptor> LoadDescriptors();

        /// <summary>
        /// 存储依赖项描述符。
        /// </summary>
        /// <param name="dependencyDescriptors">依赖项描述符集合。</param>
        void StoreDescriptors(IEnumerable<DependencyDescriptor> dependencyDescriptors);
    }
}