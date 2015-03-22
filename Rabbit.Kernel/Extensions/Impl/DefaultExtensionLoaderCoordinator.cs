using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Extensions.Loaders;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.Dependencies;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.Kernel.Extensions.Impl
{
    internal sealed class DefaultExtensionLoaderCoordinator : IExtensionLoaderCoordinator
    {
        #region Field

        private readonly IDependenciesFolder _dependenciesFolder;
        private readonly IExtensionDependenciesManager _extensionDependenciesManager;
        private readonly IExtensionManager _extensionManager;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IEnumerable<IExtensionLoader> _loaders;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IParallelCacheContext _parallelCacheContext;
        private readonly IBuildManager _buildManager;

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        public DefaultExtensionLoaderCoordinator(
            IDependenciesFolder dependenciesFolder,
            IExtensionDependenciesManager extensionDependenciesManager,
            IExtensionManager extensionManager,
            IVirtualPathProvider virtualPathProvider,
            IEnumerable<IExtensionLoader> loaders,
            IHostEnvironment hostEnvironment,
            IParallelCacheContext parallelCacheContext,
            IBuildManager buildManager)
        {
            _dependenciesFolder = dependenciesFolder;
            _extensionDependenciesManager = extensionDependenciesManager;
            _extensionManager = extensionManager;
            _virtualPathProvider = virtualPathProvider;
            _loaders = loaders.OrderBy(l => l.Order).ToArray();
            _hostEnvironment = hostEnvironment;
            _parallelCacheContext = parallelCacheContext;
            _buildManager = buildManager;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IExtensionLoaderCoordinator

        /// <summary>
        /// 安装扩展。
        /// </summary>
        public void SetupExtensions()
        {
            Logger.Information("开始加载扩展...");

            var context = CreateLoadingContext();

            //通知应用程序中的所有装载机有关扩展被删除
            foreach (var dependency in context.DeletedDependencies)
            {
                Logger.Information("扩展 {0} 已从应用程序中删除", dependency.Name);
                var dependencyProxy = dependency;
                foreach (var loader in _loaders.Where(loader => dependencyProxy.LoaderName == loader.Name))
                {
                    loader.ExtensionRemoved(context, dependency);
                }
            }

            //要求所有装载机对应用程序中所有现有可用的扩展进行加载。
            foreach (var extension in context.AvailableExtensions)
                ProcessExtension(context, extension);

            //执行所有需要通过上下文的工作
            ProcessContextCommands(context);

            //保存最新的条目中的依赖关系
            _dependenciesFolder.StoreDescriptors(context.NewDependencies);
            _extensionDependenciesManager.StoreDependencies(context.NewDependencies, desc => GetExtensionHash(context, desc));

            Logger.Information("扩展加载完成");

            //最后一步：如果需要，通知主机环境来重新启动AppDomain。
            if (!context.RestartAppDomain)
                return;
            Logger.Information("AppDomain 需要重新启动。");
            _hostEnvironment.RestartAppDomain();
        }

        #endregion Implementation of IExtensionLoaderCoordinator

        #region Private Method

        private string GetExtensionHash(ExtensionLoadingContext context, DependencyDescriptor dependencyDescriptor)
        {
            var hash = new Hash();
            hash.AddStringInvariant(dependencyDescriptor.Name);

            foreach (var virtualpathDependency in context.ProcessedExtensions[dependencyDescriptor.Name].VirtualPathDependencies)
            {
                hash.AddDateTime(GetVirtualPathModificationTimeUtc(context.VirtualPathModficationDates, virtualpathDependency));
            }

            foreach (var reference in dependencyDescriptor.References)
            {
                hash.AddStringInvariant(reference.Name);
                hash.AddString(reference.LoaderName);
                hash.AddDateTime(GetVirtualPathModificationTimeUtc(context.VirtualPathModficationDates, reference.VirtualPath));
            }

            return hash.Value;
        }

        private void ProcessExtension(ExtensionLoadingContext context, ExtensionDescriptorEntry extension)
        {
            var extensionProbes = context.AvailableExtensionsProbes.ContainsKey(extension.Id) ?
                context.AvailableExtensionsProbes[extension.Id] :
                Enumerable.Empty<ExtensionProbeEntry>();

            var extensionProbeEntries = extensionProbes as ExtensionProbeEntry[] ?? extensionProbes.ToArray();
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.Debug("加载扩展 \"{0}\": ", extension.Id);
                foreach (var probe in extensionProbeEntries)
                {
                    Logger.Debug("  Loader: {0}", probe.Loader.Name);
                    Logger.Debug("    VirtualPath: {0}", probe.VirtualPath);
                    Logger.Debug("    VirtualPathDependencies: {0}", string.Join(", ", probe.VirtualPathDependencies));
                }
            }

            var moduleReferences =
                context.AvailableExtensions
                    .Where(e =>
                           context.ReferencesByModule.ContainsKey(extension.Id) &&
                           context.ReferencesByModule[extension.Id].Any(r => StringComparer.OrdinalIgnoreCase.Equals(e.Id, r.Name)))
                    .ToList();

            var processedModuleReferences =
                moduleReferences
                .Where(e => context.ProcessedExtensions.ContainsKey(e.Id))
                .Select(e => context.ProcessedExtensions[e.Id])
                .ToList();

            var activatedExtension = extensionProbeEntries.FirstOrDefault(
                e => e.Loader.IsCompatibleWithModuleReferences(extension, processedModuleReferences)
                );

            var previousDependency = context.PreviousDependencies.FirstOrDefault(
                d => StringComparer.OrdinalIgnoreCase.Equals(d.Name, extension.Id)
                );

            if (activatedExtension == null)
            {
                Logger.Warning("没有找到装载机来装载扩展 \"{0}\"!", extension.Id);
            }

            var references = ProcessExtensionReferences(context, activatedExtension);

            foreach (var loader in _loaders)
            {
                if (activatedExtension != null && activatedExtension.Loader.Name == loader.Name)
                {
                    Logger.Information("使用装载机 \"{1}\" 来激活扩展 \"{0}\"", activatedExtension.Descriptor.Id, loader.Name);
                    loader.ExtensionActivated(context, extension);
                }
                else if (previousDependency != null && previousDependency.LoaderName == loader.Name)
                {
                    Logger.Information("使用装载机 \"{1}\" 来停用扩展 \"{0}\"", previousDependency.Name, loader.Name);
                    loader.ExtensionDeactivated(context, extension);
                }
            }

            if (activatedExtension != null)
            {
                context.NewDependencies.Add(new DependencyDescriptor
                {
                    Name = extension.Id,
                    LoaderName = activatedExtension.Loader.Name,
                    VirtualPath = activatedExtension.VirtualPath,
                    References = references
                });
            }

            //跟踪哪些装载机，我们使用的每一个扩展
            //这将需要从其他相关的扩展处理参考
            context.ProcessedExtensions.Add(extension.Id, activatedExtension);
        }

        private ExtensionLoadingContext CreateLoadingContext()
        {
            var availableExtensions = _extensionManager
                .AvailableExtensions()
                //                .Where(d => DefaultExtensionTypes.IsModule(d.ExtensionType) || DefaultExtensionTypes.IsTheme(d.ExtensionType))
                .OrderBy(d => d.Id)
                .ToList();

            //检查重复项
            var duplicates = availableExtensions.GroupBy(ed => ed.Id).Where(g => g.Count() >= 2).ToList();
            if (duplicates.Any())
            {
                var sb = new StringBuilder();
                sb.Append("有多个具有相同名称的扩展。\r\n");
                foreach (var dup in duplicates)
                {
                    sb.AppendFormat("扩展 '{0}' 从下列位置已被发现: {1}.\r\n", dup.Key, string.Join(", ", dup.Select(e => e.Location + "/" + e.Id)));
                }
                sb.Append("这个问题通常可以通过删除或重命名冲突的扩展解决。");
                var message = sb.ToString();
                Logger.Error(message);
                throw new RabbitException(new LocalizedString(message));
            }

            var previousDependencies = _dependenciesFolder.LoadDescriptors().ToList();

            var virtualPathModficationDates = new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

            Logger.Information("准备探测扩展");
            var availableExtensionsProbes1 = _parallelCacheContext
                .RunInParallel(availableExtensions, extension =>
                    _loaders.Select(loader => loader.Probe(extension)).Where(entry => entry != null).ToArray())
                .SelectMany(entries => entries)
                .GroupBy(entry => entry.Descriptor.Id);

            var availableExtensionsProbes = _parallelCacheContext
                .RunInParallel(availableExtensionsProbes1, g =>
                    new { Id = g.Key, Entries = SortExtensionProbeEntries(g, virtualPathModficationDates) })
                .ToDictionary(g => g.Id, g => g.Entries, StringComparer.OrdinalIgnoreCase);
            Logger.Information("探测扩展完成");

            var deletedDependencies = previousDependencies
                .Where(e => !availableExtensions.Any(e2 => StringComparer.OrdinalIgnoreCase.Equals(e2.Id, e.Name)))
                .ToList();

            //收集所有模块中的引用
            Logger.Information("准备探测扩展引用");
            var references = _parallelCacheContext
                .RunInParallel(availableExtensions, extension => _loaders.SelectMany(loader => loader.ProbeReferences(extension)).ToList())
                .SelectMany(entries => entries)
                .ToList();
            Logger.Information("探测扩展引用完成");

            var referencesByModule = references
                .GroupBy(entry => entry.Descriptor.Id, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.AsEnumerable(), StringComparer.OrdinalIgnoreCase);

            var referencesByName = references
                .GroupBy(reference => reference.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.AsEnumerable(), StringComparer.OrdinalIgnoreCase);

            var sortedAvailableExtensions =
                availableExtensions.OrderByDependenciesAndPriorities(
                    (item, dep) => referencesByModule.ContainsKey(item.Id) &&
                                   referencesByModule[item.Id].Any(r => StringComparer.OrdinalIgnoreCase.Equals(dep.Id, r.Name)),
                    item => 0)
                    .ToList();

            return new ExtensionLoadingContext
            {
                AvailableExtensions = sortedAvailableExtensions,
                PreviousDependencies = previousDependencies,
                DeletedDependencies = deletedDependencies,
                AvailableExtensionsProbes = availableExtensionsProbes,
                ReferencesByName = referencesByName,
                ReferencesByModule = referencesByModule,
                VirtualPathModficationDates = virtualPathModficationDates,
            };
        }

        private IEnumerable<ExtensionProbeEntry> SortExtensionProbeEntries(IEnumerable<ExtensionProbeEntry> entries, ConcurrentDictionary<string, DateTime> virtualPathModficationDates)
        {
            //所有"entries"是相同的扩展名ID，所以我们只需要筛选/排序优先级+修改日期。
            var groupByPriority = entries
                .GroupBy(entry => entry.Priority)
                .OrderByDescending(g => g.Key);

            //至少有一个项目，选择最高优先级组
            var firstNonEmptyGroup = groupByPriority.FirstOrDefault(g => g.Any()) ?? Enumerable.Empty<ExtensionProbeEntry>();

            //落实列表。
            var sortExtensionProbeEntries = firstNonEmptyGroup as ExtensionProbeEntry[] ?? firstNonEmptyGroup.ToArray();

            //如果只有1个项目发现无需进一步分拣

            if (sortExtensionProbeEntries.Count() <= 1)
                return sortExtensionProbeEntries;

            // 最后修改日期/装载机订单排序
            return sortExtensionProbeEntries
                .OrderByDescending(probe => GetVirtualPathDepedenciesModificationTimeUtc(virtualPathModficationDates, probe))
                .ThenBy(probe => probe.Loader.Order)
                .ToList();
        }

        private DateTime? GetVirtualPathDepedenciesModificationTimeUtc(ConcurrentDictionary<string, DateTime> virtualPathDependencies, ExtensionProbeEntry probe)
        {
            if (!probe.VirtualPathDependencies.Any())
                return null;

            Logger.Information("正在修改扩展 '{0}' 依赖项的日期", probe.Descriptor.Id);

            var result = probe.VirtualPathDependencies.Max(path => GetVirtualPathModificationTimeUtc(virtualPathDependencies, path));

            Logger.Information("完成扩展 '{0}' 依赖项的日期修改", probe.Descriptor.Id);
            return result;
        }

        private DateTime GetVirtualPathModificationTimeUtc(ConcurrentDictionary<string, DateTime> virtualPathDependencies, string path)
        {
            return virtualPathDependencies.GetOrAdd(path, p =>
            {
                var value = _virtualPathProvider.GetFileLastWriteTimeUtc(p);
                return !value.HasValue ? DateTime.MinValue : value.Value;
            });
        }

        private IEnumerable<DependencyReferenceDescriptor> ProcessExtensionReferences(ExtensionLoadingContext context, ExtensionProbeEntry activatedExtension)
        {
            if (activatedExtension == null)
                return Enumerable.Empty<DependencyReferenceDescriptor>();

            var referenceNames = (context.ReferencesByModule.ContainsKey(activatedExtension.Descriptor.Id) ?
                context.ReferencesByModule[activatedExtension.Descriptor.Id] :
                Enumerable.Empty<ExtensionReferenceProbeEntry>())
                .Select(r => r.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase);

            var referencesDecriptors = new List<DependencyReferenceDescriptor>();
            foreach (var referenceName in referenceNames)
            {
                ProcessExtensionReference(context, referenceName, referencesDecriptors);
            }

            return referencesDecriptors;
        }

        private void ProcessExtensionReference(ExtensionLoadingContext context, string referenceName, ICollection<DependencyReferenceDescriptor> activatedReferences)
        {
            //如果参考是一个扩展已经被处理，使用相同的装载机扩展，因为给定的扩展名应具有独特的装载机在整个应用程序加载
            var bestExtensionReference = context.ProcessedExtensions.ContainsKey(referenceName) ?
                context.ProcessedExtensions[referenceName] :
                null;

            //激活扩展引用
            if (bestExtensionReference != null)
            {
                activatedReferences.Add(new DependencyReferenceDescriptor
                {
                    LoaderName = bestExtensionReference.Loader.Name,
                    Name = referenceName,
                    VirtualPath = bestExtensionReference.VirtualPath
                });

                return;
            }

            //跳过来自 "~/bin" 的引用。
            if (_buildManager.HasReferencedAssembly(referenceName))
                return;

            //二进制引用
            var references = context.ReferencesByName.ContainsKey(referenceName) ?
                context.ReferencesByName[referenceName] :
                Enumerable.Empty<ExtensionReferenceProbeEntry>();

            var bestBinaryReference = references
                .Where(entry => !string.IsNullOrEmpty(entry.VirtualPath))
                .Select(entry => new { Entry = entry, LastWriteTimeUtc = _virtualPathProvider.GetFileLastWriteTimeUtc(entry.VirtualPath) })
                .OrderBy(e => e.LastWriteTimeUtc)
                .ThenBy(e => e.Entry.Name)
                .FirstOrDefault();

            //激活二进制引用
            if (bestBinaryReference == null)
                return;

            if (!context.ProcessedReferences.ContainsKey(bestBinaryReference.Entry.Name))
            {
                context.ProcessedReferences.Add(bestBinaryReference.Entry.Name, bestBinaryReference.Entry);
                bestBinaryReference.Entry.Loader.ReferenceActivated(context, bestBinaryReference.Entry);
            }
            activatedReferences.Add(new DependencyReferenceDescriptor
            {
                LoaderName = bestBinaryReference.Entry.Loader.Name,
                Name = bestBinaryReference.Entry.Name,
                VirtualPath = bestBinaryReference.Entry.VirtualPath
            });
        }

        private void ProcessContextCommands(ExtensionLoadingContext ctx)
        {
            Logger.Information("执行的加载扩展所需的操作列表...");
            foreach (var action in ctx.DeleteActions)
            {
                action();
            }

            foreach (var action in ctx.CopyActions)
            {
                action();
            }
        }

        #endregion Private Method
    }
}