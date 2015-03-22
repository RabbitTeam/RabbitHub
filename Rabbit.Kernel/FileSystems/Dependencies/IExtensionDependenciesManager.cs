using Rabbit.Kernel.Caching;
using System;
using System.Collections.Generic;

namespace Rabbit.Kernel.FileSystems.Dependencies
{
    /// <summary>
    /// 已经激活的扩展描述符。
    /// </summary>
    public class ActivatedExtensionDescriptor
    {
        /// <summary>
        /// 扩展Id。
        /// </summary>
        public string ExtensionId { get; set; }

        /// <summary>
        /// 装载机名称。
        /// </summary>
        public string LoaderName { get; set; }

        /// <summary>
        /// 虚拟路径。
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// 哈希值。
        /// </summary>
        public string Hash { get; set; }
    }

    /// <summary>
    /// 一个抽象的扩展依管理者。
    /// </summary>
    public interface IExtensionDependenciesManager : IVolatileProvider
    {
        /// <summary>
        /// 存储依赖。
        /// </summary>
        /// <param name="dependencyDescriptors">依赖描述符集合。</param>
        /// <param name="fileHashProvider">文件哈希值提供程序。</param>
        void StoreDependencies(IEnumerable<DependencyDescriptor> dependencyDescriptors, Func<DependencyDescriptor, string> fileHashProvider);

        /// <summary>
        /// 获取扩展的虚拟路径依赖项。
        /// </summary>
        /// <param name="extensionId">扩展Id。</param>
        /// <returns>虚拟路径集合。</returns>
        IEnumerable<string> GetVirtualPathDependencies(string extensionId);

        /// <summary>
        /// 获取一个已经激活的扩展描述符。
        /// </summary>
        /// <param name="extensionId">扩展Id。</param>
        /// <returns>已经激活的扩展描述符。</returns>
        ActivatedExtensionDescriptor GetDescriptor(string extensionId);
    }
}