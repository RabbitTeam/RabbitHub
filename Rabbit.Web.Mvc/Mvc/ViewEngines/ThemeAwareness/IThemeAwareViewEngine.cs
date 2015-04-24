using Rabbit.Kernel;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness
{
    internal interface IThemeAwareViewEngine : IDependency
    {
        ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache, bool useDeepPaths);

        ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache, bool useDeepPaths);
    }
}