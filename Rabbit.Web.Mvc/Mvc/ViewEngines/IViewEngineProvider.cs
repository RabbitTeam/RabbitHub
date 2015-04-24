using Rabbit.Kernel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines
{
    internal sealed class CreateThemeViewEngineParams
    {
        public string VirtualPath { get; set; }
    }

    internal sealed class CreateModulesViewEngineParams
    {
        public IEnumerable<string> VirtualPaths { get; set; }
    }

    /// <summary>
    /// 一个抽象的视图引擎提供程序。
    /// </summary>
    internal interface IViewEngineProvider : ISingletonDependency
    {
        /// <summary>
        /// 创建主题视图引擎。
        /// </summary>
        /// <param name="parameters">创建主题视图引擎所需的参数。</param>
        /// <returns>视图引擎。</returns>
        IViewEngine CreateThemeViewEngine(CreateThemeViewEngineParams parameters);

        /// <summary>
        /// 创建模块视图引擎。
        /// </summary>
        /// <param name="parameters">创建模块视图引擎所需的参数。</param>
        /// <returns>视图引擎。</returns>
        IViewEngine CreateModulesViewEngine(CreateModulesViewEngineParams parameters);

        /// <summary>
        /// 创建一个基本的视图引擎。
        /// </summary>
        /// <returns>视图引擎。</returns>
        IViewEngine CreateBareViewEngine();
    }
}