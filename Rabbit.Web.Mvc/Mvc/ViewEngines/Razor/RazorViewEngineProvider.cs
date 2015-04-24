using Rabbit.Kernel.Logging;
using Rabbit.Web.Mvc.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.Razor
{
    internal sealed class RazorViewEngineProvider : IViewEngineProvider, IShapeTemplateViewEngine
    {
        public ILogger Logger { get; set; }

        public RazorViewEngineProvider()
        {
            Logger = NullLogger.Instance;
            RazorCompilationEventsShim.EnsureInitialized();
        }

        private static readonly string[] DisabledFormats = new[] { "~/Disabled" };

        #region Implementation of IViewEngineProvider

        /// <summary>
        /// 创建主题视图引擎。
        /// </summary>
        /// <param name="parameters">创建主题视图引擎所需的参数。</param>
        /// <returns>视图引擎。</returns>
        public IViewEngine CreateThemeViewEngine(CreateThemeViewEngineParams parameters)
        {
            // /Views/{area}/{controller}/{viewName}

            // /Views/{partialName}
            // /Views/"DisplayTemplates/"+{templateName}
            // /Views/"EditorTemplates/+{templateName}
            var partialViewLocationFormats = new[] {
                parameters.VirtualPath + "/Views/{0}.cshtml",
            };

            var areaPartialViewLocationFormats = new[] {
                parameters.VirtualPath + "/Views/{2}/{1}/{0}.cshtml",
            };

            var viewEngine = new RazorViewEngine
            {
                MasterLocationFormats = DisabledFormats,
                ViewLocationFormats = DisabledFormats,
                PartialViewLocationFormats = partialViewLocationFormats,
                AreaMasterLocationFormats = DisabledFormats,
                AreaViewLocationFormats = DisabledFormats,
                AreaPartialViewLocationFormats = areaPartialViewLocationFormats,
                ViewLocationCache = new ThemeViewLocationCache(parameters.VirtualPath),
            };

            return viewEngine;
        }

        /// <summary>
        /// 创建模块视图引擎。
        /// </summary>
        /// <param name="parameters">创建模块视图引擎所需的参数。</param>
        /// <returns>视图引擎。</returns>
        public IViewEngine CreateModulesViewEngine(CreateModulesViewEngineParams parameters)
        {
            var areaFormats = new[] {
                                        "~/Core/{2}/Views/{1}/{0}.cshtml",
                                        "~/Modules/{2}/Views/{1}/{0}.cshtml",
                                        "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                    };

            var universalFormats = parameters.VirtualPaths
                .SelectMany(x => new[] {
                                           x + "/Views/{0}.cshtml",
                                       })
                .ToArray();

            var viewEngine = new RazorViewEngine
            {
                MasterLocationFormats = DisabledFormats,
                ViewLocationFormats = universalFormats,
                PartialViewLocationFormats = universalFormats,
                AreaMasterLocationFormats = DisabledFormats,
                AreaViewLocationFormats = areaFormats,
                AreaPartialViewLocationFormats = areaFormats,
            };

            return viewEngine;
        }

        /// <summary>
        /// 创建一个基本的视图引擎。
        /// </summary>
        /// <returns>视图引擎。</returns>
        public IViewEngine CreateBareViewEngine()
        {
            return new RazorViewEngine
            {
                MasterLocationFormats = DisabledFormats,
                ViewLocationFormats = DisabledFormats,
                PartialViewLocationFormats = DisabledFormats,
                AreaMasterLocationFormats = DisabledFormats,
                AreaViewLocationFormats = DisabledFormats,
                AreaPartialViewLocationFormats = DisabledFormats,
            };
        }

        #endregion Implementation of IViewEngineProvider

        #region Implementation of IShapeTemplateViewEngine

        /// <summary>
        /// 检测模板文件名称。
        /// </summary>
        /// <param name="fileNames">文件名称集合。</param>
        /// <returns>文件名称集合。</returns>
        public IEnumerable<string> DetectTemplateFileNames(IEnumerable<string> fileNames)
        {
            return fileNames.Where(fileName => fileName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase));
        }

        #endregion Implementation of IShapeTemplateViewEngine
    }
}