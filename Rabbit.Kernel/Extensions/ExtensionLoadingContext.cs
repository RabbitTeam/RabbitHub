using Rabbit.Kernel.Extensions.Loaders;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.Dependencies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rabbit.Kernel.Extensions
{
    /// <summary>
    /// 扩展装载上下文。
    /// </summary>
    public sealed class ExtensionLoadingContext
    {
        /// <summary>
        /// 初始化一个新的扩展装载上下文实例。
        /// </summary>
        public ExtensionLoadingContext()
        {
            ProcessedExtensions = new Dictionary<string, ExtensionProbeEntry>(StringComparer.OrdinalIgnoreCase);
            ProcessedReferences = new Dictionary<string, ExtensionReferenceProbeEntry>(StringComparer.OrdinalIgnoreCase);
            DeleteActions = new List<Action>();
            CopyActions = new List<Action>();
            NewDependencies = new List<DependencyDescriptor>();
        }

        /// <summary>
        /// 正在运行的扩展。
        /// </summary>
        public IDictionary<string, ExtensionProbeEntry> ProcessedExtensions { get; private set; }

        /// <summary>
        /// 正在运行的引用。
        /// </summary>
        public IDictionary<string, ExtensionReferenceProbeEntry> ProcessedReferences { get; private set; }

        /// <summary>
        /// 新的依赖信息集合。
        /// </summary>
        public IList<DependencyDescriptor> NewDependencies { get; private set; }

        /// <summary>
        /// 删除动作。
        /// </summary>
        public IList<Action> DeleteActions { get; private set; }

        /// <summary>
        /// 复制动作。
        /// </summary>
        public IList<Action> CopyActions { get; private set; }

        /// <summary>
        /// 是否需要重新启动AppDomain。
        /// </summary>
        public bool RestartAppDomain { get; set; }

        /// <summary>
        /// 跟踪文件的修改日期 (VirtualPath => DateTime)
        /// </summary>
        public ConcurrentDictionary<string, DateTime> VirtualPathModficationDates { get; set; }

        /// <summary>
        /// 在系统中正在运行的扩展。
        /// </summary>
        public List<ExtensionDescriptorEntry> AvailableExtensions { get; set; }

        /// <summary>
        /// 之前成功运行的扩展。
        /// </summary>
        public List<DependencyDescriptor> PreviousDependencies { get; set; }

        /// <summary>
        /// 需要删除的扩展。
        /// </summary>
        public List<DependencyDescriptor> DeletedDependencies { get; set; }

        /// <summary>
        /// 可用的扩展装载机。
        /// </summary>
        public IDictionary<string, IEnumerable<ExtensionProbeEntry>> AvailableExtensionsProbes { get; set; }

        /// <summary>
        /// 按模块分组的引用信息。
        /// </summary>
        public IDictionary<string, IEnumerable<ExtensionReferenceProbeEntry>> ReferencesByModule { get; set; }

        /// <summary>
        /// 按引用名称分组的引用信息。
        /// </summary>
        public IDictionary<string, IEnumerable<ExtensionReferenceProbeEntry>> ReferencesByName { get; set; }
    }
}