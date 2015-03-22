using Autofac.Core;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Environment.ShellBuilders.Models;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Environment.ShellBuilders.Impl
{
    internal class DefaultCompositionStrategy : ICompositionStrategy
    {
        #region Field

        private readonly IExtensionManager _extensionManager;
        private readonly IServiceTypeHarvester _serviceTypeHarvester;
        private readonly IEnumerable<ICompositionStrategyProvider> _compositionStrategyProviders;

        #endregion Field

        #region Constructor

        public DefaultCompositionStrategy(IExtensionManager extensionManager, IServiceTypeHarvester serviceTypeHarvester, IEnumerable<ICompositionStrategyProvider> compositionStrategyProviders)
        {
            _extensionManager = extensionManager;
            _serviceTypeHarvester = serviceTypeHarvester;
            _compositionStrategyProviders = compositionStrategyProviders;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of ICompositionStrategy

        /// <summary>
        /// 组合外壳蓝图。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <param name="descriptor">外壳描述符。</param>
        /// <returns>外壳蓝图。</returns>
        public ShellBlueprint Compose(ShellSettings settings, ShellDescriptor descriptor)
        {
            Logger.Debug("组合外壳蓝图");

            var enabledFeatures = _extensionManager.EnabledFeatures(descriptor);
            var features = _extensionManager.LoadFeatures(enabledFeatures).ToArray();

            if (descriptor.Features.Any(feature => feature.Name == "Rabbit.Kernel"))
                features = features.Concat(BuiltinFeatures()).ToArray();

            var excludedTypes = GetExcludedTypes(features).ToArray();

            var modules = BuildBlueprint(features, IsModule, BuildModule, excludedTypes);
            var dependencies = BuildBlueprint(features, IsDependency, BuildDependency, excludedTypes);

            var result = new ShellBlueprint
            {
                Settings = settings,
                Descriptor = descriptor,
                Dependencies = dependencies.Concat(modules).ToArray()
            };

            Logger.Debug("准备应用外部组合策略。");
            var context = new CompositionStrategyApplyContext
            {
                Features = features,
                ExcludedTypes = excludedTypes,
                ShellBlueprint = result
            };
            _compositionStrategyProviders.Invoke(i => i.Apply(context), Logger);
            Logger.Debug("应用外部组合策略成功。");

            Logger.Debug("组合外壳蓝图完成。");
            return result;
        }

        #endregion Implementation of ICompositionStrategy

        #region Private Method

        private static IEnumerable<string> GetExcludedTypes(IEnumerable<Feature> features)
        {
            var excludedTypes = new HashSet<string>();

            //标识需要被替换的类型。
            foreach (var replacedType in features.SelectMany(feature => feature.ExportedTypes.SelectMany(type => type.GetCustomAttributes(typeof(SuppressDependencyAttribute), false).Cast<SuppressDependencyAttribute>())))
                excludedTypes.Add(replacedType.FullName);

            return excludedTypes;
        }

        private IEnumerable<Feature> BuiltinFeatures()
        {
            yield return new Feature
            {
                Descriptor = new FeatureDescriptor
                {
                    Id = "Rabbit.Kernel",
                    Extension = new ExtensionDescriptorEntry(new ExtensionDescriptor(), "Rabbit.Kernel", "Module", string.Empty)
                },
                ExportedTypes = _serviceTypeHarvester.GetTypes(typeof(IDependency).Assembly)
            };
        }

        private static IEnumerable<T> BuildBlueprint<T>(
            IEnumerable<Feature> features,
            Func<Type, bool> predicate,
            Func<Type, Feature, T> selector,
            IEnumerable<string> excludedTypes)
        {
            //加载类型排除替被换的类型
            return features.SelectMany(
                feature => feature.ExportedTypes
                               .Where(predicate)
                               .Where(type => !excludedTypes.Contains(type.FullName))
                               .Select(type => selector(type, feature)))
                .ToArray();
        }

        private static bool IsModule(Type type)
        {
            return typeof(IModule).IsAssignableFrom(type);
        }

        private static DependencyBlueprintItem BuildModule(Type type, Feature feature)
        {
            return new DependencyBlueprintItem { Type = type, Feature = feature };
        }

        private static bool IsDependency(Type type)
        {
            return typeof(IDependency).IsAssignableFrom(type);
        }

        private static DependencyBlueprintItem BuildDependency(Type type, Feature feature)
        {
            return new DependencyBlueprintItem
            {
                Type = type,
                Feature = feature
            };
        }

        #endregion Private Method
    }
}