using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Extensions.Folders;
using Rabbit.Kernel.Extensions.Loaders;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Extensions.Impl
{
    internal sealed class DefaultExtensionManager : IExtensionManager
    {
        #region Field

        private readonly ICacheManager _cacheManager;
        private readonly IEnumerable<IExtensionFolders> _folders;
        private readonly IParallelCacheContext _parallelCacheContext;
        private readonly IEnumerable<IExtensionDescriptorFilter> _extensionDescriptorFilters;
        private readonly IEnumerable<IFeatureDescriptorFilter> _featureDescriptorFilters;
        private readonly IAsyncTokenProvider _asyncTokenProvider;
        private readonly Lazy<IEnumerable<IExtensionLoader>> _loaders;
        private readonly IServiceTypeHarvester _serviceTypeHarvester;

        #endregion Field

        #region Property

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        public DefaultExtensionManager(
            IEnumerable<IExtensionFolders> folders,
            ICacheManager cacheManager,
            IParallelCacheContext parallelCacheContext,
            IEnumerable<IExtensionDescriptorFilter> extensionDescriptorFilters,
            IEnumerable<IFeatureDescriptorFilter> featureDescriptorFilters,
            IAsyncTokenProvider asyncTokenProvider,
            Lazy<IEnumerable<IExtensionLoader>> loaders,
            IServiceTypeHarvester serviceTypeHarvester)
        {
            _folders = folders;
            _cacheManager = cacheManager;
            _parallelCacheContext = parallelCacheContext;
            _extensionDescriptorFilters = extensionDescriptorFilters;
            _featureDescriptorFilters = featureDescriptorFilters;
            _asyncTokenProvider = asyncTokenProvider;
            _loaders = loaders;
            _serviceTypeHarvester = serviceTypeHarvester;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IExtensionManager

        /// <summary>
        ///     可用的扩展。
        /// </summary>
        /// <returns>扩展描述符条目集合。</returns>
        public IEnumerable<ExtensionDescriptorEntry> AvailableExtensions()
        {
            return _cacheManager.Get("AvailableExtensions", ctx => _parallelCacheContext
                .RunInParallel(_folders, folder => folder.AvailableExtensions().Where(i => _extensionDescriptorFilters.All(f =>
                {
                    var context = new ExtensionDescriptorEntryFilterContext(i);
                    f.OnDiscovery(context);
                    return context.Valid;
                })).ToArray())
                .SelectMany(entrys => entrys).ToArray());
        }

        /// <summary>
        ///     可用的特性。
        /// </summary>
        /// <returns>特性描述符集合。</returns>
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            return _cacheManager.Get("AvailableFeatures", ctx => AvailableExtensions()
                .SelectMany(ext => ext.Descriptor.Features)
                .Where(i => _featureDescriptorFilters.All(f =>
                {
                    var context = new FeatureDescriptorFilterContext(i);
                    f.OnDiscovery(context);
                    return context.Valid;
                }))
                .OrderByDependenciesAndPriorities(HasDependency, GetPriority)
                .ToArray());
        }

        /// <summary>
        ///     根据扩展Id获取指定的扩展描述符。
        /// </summary>
        /// <param name="id">扩展Id。</param>
        /// <returns>扩展描述符条目。</returns>
        public ExtensionDescriptorEntry GetExtension(string id)
        {
            return AvailableExtensions().FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        ///     加载特性。
        /// </summary>
        /// <param name="featureDescriptors">特性描述符。</param>
        /// <returns>特性集合。</returns>
        public IEnumerable<Feature> LoadFeatures(IEnumerable<FeatureDescriptor> featureDescriptors)
        {
            Logger.Information("加载特性");

            var result =
                _parallelCacheContext
                .RunInParallel(featureDescriptors, descriptor => _cacheManager.Get(descriptor.Id, ctx => LoadFeature(descriptor)))
                .ToArray();

            Logger.Information("完成特性加载");
            return result;
        }

        #endregion Implementation of IExtensionManager

        #region Private Method

        private static bool HasDependency(FeatureDescriptor item, FeatureDescriptor subject)
        {
            return item.Dependencies != null &&
                   item.Dependencies.Any(x => StringComparer.OrdinalIgnoreCase.Equals(x, subject.Id));
        }

        private static int GetPriority(FeatureDescriptor featureDescriptor)
        {
            return featureDescriptor.Priority;
        }

        private Feature LoadFeature(FeatureDescriptor featureDescriptor)
        {
            var extensionDescriptor = featureDescriptor.Extension;
            var featureId = featureDescriptor.Id;
            var extensionId = extensionDescriptor.Id;

            ExtensionEntry extensionEntry;
            try
            {
                extensionEntry = _cacheManager.Get(extensionId, ctx =>
                {
                    var entry = BuildEntry(extensionDescriptor);
                    if (entry != null)
                    {
                        ctx.Monitor(_asyncTokenProvider.GetToken(monitor =>
                        {
                            foreach (var loader in _loaders.Value)
                            {
                                loader.Monitor(entry.Descriptor, monitor);
                            }
                        }));
                    }
                    return entry;
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "加载扩展 '{0}' 失败", extensionId);
                throw new RabbitException(T("加载扩展 '{0}' 失败。", extensionId), ex);
            }

            if (extensionEntry == null)
            {
                //如果该功能因为某种原因不能被编译，返回一个"null"的特性，即没有输出的类型的功能。
                return new Feature
                {
                    Descriptor = featureDescriptor,
                    ExportedTypes = Enumerable.Empty<Type>()
                };
            }

            var extensionTypes = _serviceTypeHarvester.GeTypes(extensionEntry.ExportedTypes);

            var featureTypes = extensionTypes.Where(
                i =>
                    string.Equals(GetSourceFeatureNameForType(i, extensionId), featureId,
                        StringComparison.OrdinalIgnoreCase)).ToArray();

            return new Feature
            {
                Descriptor = featureDescriptor,
                ExportedTypes = featureTypes
            };
        }

        private static string GetSourceFeatureNameForType(Type type, string extensionId)
        {
            foreach (FeatureAttribute featureAttribute in type.GetCustomAttributes(typeof(FeatureAttribute), false))
                return featureAttribute.FeatureName;
            return extensionId;
        }

        private ExtensionEntry BuildEntry(ExtensionDescriptorEntry descriptor)
        {
            var entry = _loaders.Value.Select(loader => loader.Load(descriptor)).FirstOrDefault(i => i != null);
            if (entry == null)
                Logger.Warning("没有发现适合扩展 \"{0}\" 的装载机", descriptor.Id);
            return entry;
        }

        #endregion Private Method
    }
}