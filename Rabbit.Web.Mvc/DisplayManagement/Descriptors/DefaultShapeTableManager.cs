using Autofac.Features.Metadata;
using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility;
using Rabbit.Kernel.Utility.Extensions;
using Rabbit.Web.Mvc.Utility.Extensions;
using Rabbit.Web.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors
{
    internal sealed class DefaultShapeTableManager : IShapeTableManager
    {
        private readonly IEnumerable<Meta<IShapeTableProvider>> _bindingStrategies;
        private readonly IExtensionManager _extensionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IParallelCacheContext _parallelCacheContext;
        private readonly IEnumerable<IShapeTableEventHandler> _shapeTableEventHandlers;

        public DefaultShapeTableManager(
            IEnumerable<Meta<IShapeTableProvider>> bindingStrategies,
            IExtensionManager extensionManager,
            ICacheManager cacheManager,
            IParallelCacheContext parallelCacheContext,
            IEnumerable<IShapeTableEventHandler> shapeTableEventHandlers
            )
        {
            _extensionManager = extensionManager;
            _cacheManager = cacheManager;
            _parallelCacheContext = parallelCacheContext;
            _shapeTableEventHandlers = shapeTableEventHandlers;
            _bindingStrategies = bindingStrategies;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ShapeTable GetShapeTable(string themeName)
        {
            return _cacheManager.Get(themeName ?? string.Empty, x =>
            {
                Logger.Information("开始建立形状表格");

                var alterationSets = _parallelCacheContext.RunInParallel(_bindingStrategies, bindingStrategy =>
                {
                    var strategyDefaultFeature = bindingStrategy.Metadata.ContainsKey("Feature") ?
                                                               (Feature)bindingStrategy.Metadata["Feature"] :
                                                               null;

                    var builder = new ShapeTableBuilder(strategyDefaultFeature);
                    bindingStrategy.Value.Discover(builder);
                    return builder.BuildAlterations().ToArray();
                });

                var alterations = alterationSets
                .SelectMany(shapeAlterations => shapeAlterations)
                .Where(alteration => IsModuleOrRequestedTheme(alteration, themeName))
                .OrderByDependenciesAndPriorities(AlterationHasDependency, GetPriority)
                .ToList();

                var descriptors = alterations.GroupBy(alteration => alteration.ShapeType, StringComparer.OrdinalIgnoreCase)
                    .Select(group => group.Aggregate(
                        new ShapeDescriptor { ShapeType = group.Key },
                        (descriptor, alteration) =>
                        {
                            alteration.Alter(descriptor);
                            return descriptor;
                        })).ToList();

                foreach (var descriptor in descriptors)
                {
                    foreach (var alteration in alterations.Where(a => a.ShapeType == descriptor.ShapeType).ToList())
                    {
                        var local = new ShapeDescriptor { ShapeType = descriptor.ShapeType };
                        alteration.Alter(local);
                        descriptor.BindingSources.Add(local.BindingSource);
                    }
                }

                var result = new ShapeTable
                {
                    Descriptors = descriptors.ToDictionary(sd => sd.ShapeType, StringComparer.OrdinalIgnoreCase),
                    Bindings = descriptors.SelectMany(sd => sd.Bindings).ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase),
                };

                _shapeTableEventHandlers.Invoke(ctx => ctx.ShapeTableCreated(result), Logger);

                Logger.Information("完成形状表格建立");
                return result;
            });
        }

        private static int GetPriority(ShapeAlteration shapeAlteration)
        {
            return shapeAlteration.Feature.Descriptor.Priority;
        }

        private static bool AlterationHasDependency(ShapeAlteration item, ShapeAlteration subject)
        {
            return HasDependency(item.Feature.Descriptor, subject.Feature.Descriptor);
        }

        private bool IsModuleOrRequestedTheme(ShapeAlteration alteration, string themeName)
        {
            if (alteration == null ||
                alteration.Feature == null ||
                alteration.Feature.Descriptor == null ||
                alteration.Feature.Descriptor.Extension == null)
            {
                return false;
            }

            var extensionType = alteration.Feature.Descriptor.Extension.ExtensionType;
            if (DefaultExtensionTypes.IsModule(extensionType))
            {
                return true;
            }

            if (!DefaultExtensionTypes.IsTheme(extensionType))
                return false;
            //从主题的改变必须是从给定的主题或一个基本主题
            var featureName = alteration.Feature.Descriptor.Id;
            return String.IsNullOrEmpty(featureName) || featureName == themeName || IsBaseTheme(featureName, themeName);
        }

        private bool IsBaseTheme(string featureName, string themeName)
        {
            //判断给定的功能是给定主题的基本主题
            var availableFeatures = _extensionManager.AvailableFeatures().ToArray();

            var themeFeature = availableFeatures.SingleOrDefault(fd => fd.Id == themeName);
            while (themeFeature != null)
            {
                var baseTheme = themeFeature.Extension.Descriptor.GetBaseTheme();
                if (String.IsNullOrEmpty(baseTheme))
                {
                    return false;
                }
                if (featureName == baseTheme)
                {
                    return true;
                }
                themeFeature = availableFeatures.SingleOrDefault(fd => fd.Id == baseTheme);
            }
            return false;
        }

        /// <summary>
        /// 如果该项目有关于这个问题的明确或隐含的依赖关系，则返回true
        /// </summary>
        internal static bool HasDependency(FeatureDescriptor item, FeatureDescriptor subject)
        {
            if (DefaultExtensionTypes.IsTheme(item.Extension.ExtensionType))
            {
                if (DefaultExtensionTypes.IsModule(subject.Extension.ExtensionType))
                {
                    //主题含蓄地依赖于模块，以确保构建和覆盖排序
                    return true;
                }

                if (DefaultExtensionTypes.IsTheme(subject.Extension.ExtensionType))
                {
                    //主题取决于另一个如果这是它的基本主题
                    return item.Extension.Descriptor.GetBaseTheme() == subject.Id;
                }
            }

            //基于显式依赖关系返回
            return item.Dependencies != null &&
                   item.Dependencies.Any(x => StringComparer.OrdinalIgnoreCase.Equals(x, subject.Id));
        }
    }
}