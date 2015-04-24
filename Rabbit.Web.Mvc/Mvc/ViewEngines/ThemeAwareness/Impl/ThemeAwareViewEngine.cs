using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Works;
using Rabbit.Web.Mvc.Utility.Extensions;
using Rabbit.Web.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness.Impl
{
    internal sealed class ThemeAwareViewEngine : IThemeAwareViewEngine
    {
        private readonly WorkContext _workContext;
        private readonly IEnumerable<IViewEngineProvider> _viewEngineProviders;
        private readonly IConfiguredEnginesCache _configuredEnginesCache;
        private readonly IExtensionManager _extensionManager;
        private readonly ShellDescriptor _shellDescriptor;
        private readonly IViewEngine _nullEngines = new ViewEngineCollectionWrapper(Enumerable.Empty<IViewEngine>());

        public ThemeAwareViewEngine(
            WorkContext workContext,
            IEnumerable<IViewEngineProvider> viewEngineProviders,
            IConfiguredEnginesCache configuredEnginesCache,
            IExtensionManager extensionManager,
            ShellDescriptor shellDescriptor)
        {
            _workContext = workContext;
            _viewEngineProviders = viewEngineProviders;
            _configuredEnginesCache = configuredEnginesCache;
            _extensionManager = extensionManager;
            _shellDescriptor = shellDescriptor;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        #region Implementation of IThemeAwareViewEngine

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache,
            bool useDeepPaths)
        {
            var engines = _nullEngines;

            if (partialViewName.StartsWith("/") || partialViewName.StartsWith("~"))
            {
                engines = BareEngines();
            }
            else if (_workContext.GetCurrentTheme() != null)
            {
                engines = useDeepPaths ? DeepEngines(_workContext.GetCurrentTheme()) : ShallowEngines(_workContext.GetCurrentTheme());
            }

            return engines.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache,
            bool useDeepPaths)
        {
            var engines = _nullEngines;

            if (viewName.StartsWith("/") || viewName.StartsWith("~"))
            {
                engines = BareEngines();
            }
            else if (_workContext.GetCurrentTheme() != null)
            {
                engines = useDeepPaths ? DeepEngines(_workContext.GetCurrentTheme()) : ShallowEngines(_workContext.GetCurrentTheme());
            }

            return engines.FindView(controllerContext, viewName, masterName, useCache);
        }

        #endregion Implementation of IThemeAwareViewEngine

        #region Private Method

        private IViewEngine BareEngines()
        {
            return _configuredEnginesCache.BindBareEngines(() => new ViewEngineCollectionWrapper(_viewEngineProviders.Select(vep => vep.CreateBareViewEngine())));
        }

        private IViewEngine ShallowEngines(ExtensionDescriptorEntry theme)
        {
            return DeepEngines(theme);
        }

        private IViewEngine DeepEngines(ExtensionDescriptorEntry theme)
        {
            return _configuredEnginesCache.BindDeepEngines(theme.Id, () =>
            {
                // 搜索视图顺序:
                // 1. 当前主图
                // 2. 基础主题
                // 3. 来自模块激活功能依赖排序

                var engines = Enumerable.Empty<IViewEngine>();
                // 1. 当前主题
                engines = engines.Concat(CreateThemeViewEngines(theme));

                // 2. 基础主题
                engines = GetBaseThemes(theme).Aggregate(engines, (current, baseTheme) => current.Concat(CreateThemeViewEngines(baseTheme)));

                // 3. 来自模块激活功能依赖排序
                var enabledModules = _extensionManager.EnabledFeatures(_shellDescriptor)
                    .Reverse()  // reverse from (C <= B <= A) to (A => B => C)
                    .Where(fd => DefaultExtensionTypes.IsModule(fd.Extension.ExtensionType)).ToArray();

                var allLocations = enabledModules.Concat(enabledModules)
                    .Select(fd => fd.Extension.Location + "/" + fd.Extension.Id)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var moduleParams = new CreateModulesViewEngineParams { VirtualPaths = allLocations };
                engines = engines.Concat(_viewEngineProviders.Select(vep => vep.CreateModulesViewEngine(moduleParams)));

                return new ViewEngineCollectionWrapper(engines);
            });
        }

        private IEnumerable<IViewEngine> CreateThemeViewEngines(ExtensionDescriptorEntry theme)
        {
            var themeLocation = theme.Location + "/" + theme.Id;
            var themeParams = new CreateThemeViewEngineParams { VirtualPath = themeLocation };
            return _viewEngineProviders.Select(vep => vep.CreateThemeViewEngine(themeParams));
        }

        private IEnumerable<ExtensionDescriptorEntry> GetBaseThemes(ExtensionDescriptorEntry themeExtension)
        {
            //TODO:这边需要重新考虑
            /*if (themeExtension.Id.Equals("TheAdmin", StringComparison.OrdinalIgnoreCase))
            {
                return _extensionManager
                    .EnabledFeatures(_shellDescriptor)
                    .Reverse()
                    .Select(fd => fd.Extension)
                    .Where(fd => DefaultExtensionTypes.IsTheme(fd.ExtensionType));
            }*/
            var availableFeatures = _extensionManager.AvailableFeatures().ToArray();
            var list = new List<ExtensionDescriptorEntry>();
            while (true)
            {
                if (themeExtension == null)
                    break;

                if (String.IsNullOrEmpty(themeExtension.Descriptor.GetBaseTheme()))
                    break;

                var baseFeature = availableFeatures.FirstOrDefault(fd => fd.Id == themeExtension.Descriptor.GetBaseTheme());
                if (baseFeature == null)
                {
                    Logger.Error("在 '{1}' 功能列表中找不到基础主题 '{0}'", themeExtension.Descriptor.GetBaseTheme(), themeExtension.Id);
                    break;
                }

                //防止潜在的无限循环
                if (list.Contains(baseFeature.Extension))
                {
                    Logger.Error("主题 '{1}' 是基础主题 '{0}' 所以被忽略", themeExtension.Descriptor.GetBaseTheme(), themeExtension.Id);
                    break;
                }

                list.Add(baseFeature.Extension);

                themeExtension = baseFeature.Extension;
            }
            return list;
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new NotImplementedException();
        }

        #endregion Private Method
    }
}