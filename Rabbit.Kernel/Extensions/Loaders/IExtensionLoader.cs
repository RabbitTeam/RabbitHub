using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.Dependencies;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rabbit.Kernel.Extensions.Loaders
{
    /// <summary>
    /// 扩展探测条目。
    /// </summary>
    public sealed class ExtensionProbeEntry
    {
        /// <summary>
        /// 扩展描述符条目。
        /// </summary>
        public ExtensionDescriptorEntry Descriptor { get; set; }

        /// <summary>
        /// 扩展装载机。
        /// </summary>
        public IExtensionLoader Loader { get; set; }

        /// <summary>
        /// 优先级。
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 虚拟路径。
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// 虚拟路径依赖项。
        /// </summary>
        public IEnumerable<string> VirtualPathDependencies { get; set; }
    }

    /// <summary>
    /// 扩展音域探测条目。
    /// </summary>
    public sealed class ExtensionReferenceProbeEntry
    {
        /// <summary>
        /// 扩展描述符条目。
        /// </summary>
        public ExtensionDescriptorEntry Descriptor { get; set; }

        /// <summary>
        /// 扩展装载机。
        /// </summary>
        public IExtensionLoader Loader { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 虚拟路径。
        /// </summary>
        public string VirtualPath { get; set; }
    }

    /// <summary>
    /// 扩展编译引用。
    /// </summary>
    public class ExtensionCompilationReference
    {
        /// <summary>
        /// 程序集名称。
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 目标生成提供程序。
        /// </summary>
        public string BuildProviderTarget { get; set; }
    }

    /// <summary>
    /// 一个抽象的扩展装载机。
    /// </summary>
    public interface IExtensionLoader
    {
        /// <summary>
        /// 排序。
        /// </summary>
        int Order { get; }

        /// <summary>
        /// 装载机名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 探测引用。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展音域探测条目集合。</returns>
        IEnumerable<ExtensionReferenceProbeEntry> ProbeReferences(ExtensionDescriptorEntry descriptor);

        /// <summary>
        /// 装载引用。
        /// </summary>
        /// <param name="reference">引用描述符。</param>
        /// <returns>程序集。</returns>
        Assembly LoadReference(DependencyReferenceDescriptor reference);

        /// <summary>
        /// 激活引用。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="referenceEntry">引用条目。</param>
        void ReferenceActivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry);

        /// <summary>
        /// 引用停用。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="referenceEntry">引用条目。</param>
        void ReferenceDeactivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry);

        /// <summary>
        ///
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <param name="references">扩展探测条目集合。</param>
        /// <returns></returns>
        bool IsCompatibleWithModuleReferences(ExtensionDescriptorEntry descriptor, IEnumerable<ExtensionProbeEntry> references);

        /// <summary>
        /// 探测。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展探测条目。</returns>
        ExtensionProbeEntry Probe(ExtensionDescriptorEntry descriptor);

        /// <summary>
        /// 装载扩展。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展条目。</returns>
        ExtensionEntry Load(ExtensionDescriptorEntry descriptor);

        /// <summary>
        /// 扩展激活。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="descriptor">扩展描述符条目。</param>
        void ExtensionActivated(ExtensionLoadingContext context, ExtensionDescriptorEntry descriptor);

        /// <summary>
        /// 扩展停用。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="descriptor">扩展描述符条目。</param>
        void ExtensionDeactivated(ExtensionLoadingContext context, ExtensionDescriptorEntry descriptor);

        /// <summary>
        /// 扩展删除。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="dependency">依赖项描述符。</param>
        void ExtensionRemoved(ExtensionLoadingContext context, DependencyDescriptor dependency);

        /// <summary>
        /// 监控扩展。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <param name="monitor">监控动作。</param>
        void Monitor(ExtensionDescriptorEntry descriptor, Action<IVolatileToken> monitor);

        /// <summary>
        /// 获取编译引用信息。
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns>扩展编译引用集合。</returns>
        IEnumerable<ExtensionCompilationReference> GetCompilationReferences(DependencyDescriptor dependency);

        /// <summary>
        /// 获取依赖文件的虚拟路径。
        /// </summary>
        /// <param name="dependency">依赖项描述符。</param>
        /// <returns>虚拟路径集合。</returns>
        IEnumerable<string> GetVirtualPathDependencies(DependencyDescriptor dependency);
    }
}