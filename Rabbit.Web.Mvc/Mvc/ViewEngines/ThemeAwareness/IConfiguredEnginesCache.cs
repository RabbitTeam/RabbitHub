using Rabbit.Kernel;
using System;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness
{
    internal interface IConfiguredEnginesCache : ISingletonDependency
    {
        IViewEngine BindBareEngines(Func<IViewEngine> factory);

        IViewEngine BindShallowEngines(string themeName, Func<IViewEngine> factory);

        IViewEngine BindDeepEngines(string themeName, Func<IViewEngine> factory);
    }
}