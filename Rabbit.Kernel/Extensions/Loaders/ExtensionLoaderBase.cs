using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rabbit.Kernel.Extensions.Loaders
{
    /// <summary>
    /// 扩展装载机基类。
    /// </summary>
    public abstract class ExtensionLoaderBase : IExtensionLoader
    {
        #region Field

        private readonly IDependenciesFolder _dependenciesFolder;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的扩展装载机。
        /// </summary>
        /// <param name="dependenciesFolder">依赖文件夹。</param>
        protected ExtensionLoaderBase(IDependenciesFolder dependenciesFolder)
        {
            _dependenciesFolder = dependenciesFolder;
        }

        #endregion Constructor

        #region Implementation of IExtensionLoader

        /// <summary>
        /// 排序。
        /// </summary>
        public abstract int Order { get; }

        /// <summary>
        /// 装载机名称。
        /// </summary>
        public string Name { get { return GetType().Name; } }

        /// <summary>
        /// 探测引用。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展音域探测条目集合。</returns>
        public virtual IEnumerable<ExtensionReferenceProbeEntry> ProbeReferences(ExtensionDescriptorEntry descriptor)
        {
            return Enumerable.Empty<ExtensionReferenceProbeEntry>();
        }

        /// <summary>
        /// 装载引用。
        /// </summary>
        /// <param name="reference">引用描述符。</param>
        /// <returns>程序集。</returns>
        public virtual Assembly LoadReference(DependencyReferenceDescriptor reference)
        {
            return null;
        }

        /// <summary>
        /// 激活引用。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="referenceEntry">引用条目。</param>
        public virtual void ReferenceActivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
        {
        }

        /// <summary>
        /// 引用停用。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="referenceEntry">引用条目。</param>
        public virtual void ReferenceDeactivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <param name="references">扩展探测条目集合。</param>
        /// <returns></returns>
        public virtual bool IsCompatibleWithModuleReferences(ExtensionDescriptorEntry descriptor, IEnumerable<ExtensionProbeEntry> references)
        {
            return true;
        }

        /// <summary>
        /// 探测。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展探测条目。</returns>
        public abstract ExtensionProbeEntry Probe(ExtensionDescriptorEntry descriptor);

        /// <summary>
        /// 装载扩展。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展条目。</returns>
        public ExtensionEntry Load(ExtensionDescriptorEntry descriptor)
        {
            var dependency = _dependenciesFolder.GetDescriptor(descriptor.Id);
            if (dependency != null && dependency.LoaderName == Name)
            {
                return LoadWorker(descriptor);
            }
            return null;
        }

        /// <summary>
        /// 扩展激活。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="descriptor">扩展描述符条目。</param>
        public virtual void ExtensionActivated(ExtensionLoadingContext context, ExtensionDescriptorEntry descriptor)
        {
        }

        /// <summary>
        /// 扩展停用。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="descriptor">扩展描述符条目。</param>
        public virtual void ExtensionDeactivated(ExtensionLoadingContext context, ExtensionDescriptorEntry descriptor)
        {
        }

        /// <summary>
        /// 扩展删除。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="dependency">依赖项描述符。</param>
        public virtual void ExtensionRemoved(ExtensionLoadingContext context, DependencyDescriptor dependency)
        {
        }

        /// <summary>
        /// 监控扩展。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <param name="monitor">监控动作。</param>
        public virtual void Monitor(ExtensionDescriptorEntry descriptor, Action<IVolatileToken> monitor)
        {
        }

        /// <summary>
        /// 获取编译引用信息。
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns>扩展编译引用集合。</returns>
        public virtual IEnumerable<ExtensionCompilationReference> GetCompilationReferences(DependencyDescriptor dependency)
        {
            return Enumerable.Empty<ExtensionCompilationReference>();
        }

        /// <summary>
        /// 获取依赖文件的虚拟路径。
        /// </summary>
        /// <param name="dependency">依赖项描述符。</param>
        /// <returns>虚拟路径集合。</returns>
        public virtual IEnumerable<string> GetVirtualPathDependencies(DependencyDescriptor dependency)
        {
            return Enumerable.Empty<string>();
        }

        #endregion Implementation of IExtensionLoader

        #region Abstract Method

        /// <summary>
        /// 装载工作。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展条目。</returns>
        protected abstract ExtensionEntry LoadWorker(ExtensionDescriptorEntry descriptor);

        #endregion Abstract Method
    }
}