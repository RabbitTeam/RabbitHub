using Autofac;
using Rabbit.Kernel;
using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Extensions.Folders;
using Rabbit.Kernel.Extensions.Loaders;
using Rabbit.Web.Mvc.Extensions.Folders;
using Rabbit.Web.Mvc.Extensions.Loaders;
using Rabbit.Web.Mvc.Mvc;
using Rabbit.Web.Mvc.Mvc.Filters;
using Rabbit.Web.Mvc.Mvc.ViewEngines.Razor;
using Rabbit.Web.Mvc.Mvc.ViewEngines.Razor.Impl;
using Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness;
using Rabbit.Web.Mvc.WebApi;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc
{
    /// <summary>
    ///     建设者扩展方法。
    /// </summary>
    public static class WebBuilderExtensions
    {
        /// <summary>
        ///     启用Mvc。
        /// </summary>
        /// <param name="webBuilder">Web建设者。</param>
        /// <param name="binders">模型绑定字典表，如果为 null 则默认为 ModelBinders.Binders。</param>
        /// <param name="engines">视图引擎集合，如果为 null 则默认为 ViewEngines.Engines。</param>
        /// <returns>Web 建设者。</returns>
        public static BuilderExtensions.IWebBuilder EnableMvc(this BuilderExtensions.IWebBuilder webBuilder,
            ModelBinderDictionary binders = null, ViewEngineCollection engines = null)
        {
            webBuilder.KernelBuilder
                .RegisterExtension(typeof(WebBuilderExtensions).Assembly)
                .OnStarting(builder =>
                {
                    builder.RegisterInstance(binders ?? ModelBinders.Binders);
                    builder.RegisterInstance(engines ?? ViewEngines.Engines);

                    builder.RegisterType<CompositionStrategyProvider>()
                        .As<ICompositionStrategyProvider>()
                        .SingleInstance();
                    builder.RegisterType<ShellContainerRegistrations>()
                        .As<IShellContainerRegistrations>()
                        .SingleInstance();

                    builder.RegisterType<DefaultRazorCompilationEvents>().As<IRazorCompilationEvents>().SingleInstance();
                    builder.RegisterType<CoreModuleFolders>().As<IExtensionFolders>().SingleInstance()
                        .WithParameter(new NamedParameter("paths", new[] { "~/Core" }));
                    builder.RegisterType<ThemeFolders>().As<IExtensionFolders>().SingleInstance()
                        .WithParameter("paths", new[] { "~/Themes" });
                    builder.RegisterType<CoreExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                    builder.RegisterType<RawThemeExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                })
                .OnStarted(container =>
                {
                    ControllerBuilder.Current.SetControllerFactory(new RabbitControllerFactory());
                    FilterProviders.Providers.Add(new RabbitFilterProvider());

                    GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector),
                        new DefaultWebApiHttpControllerSelector(GlobalConfiguration.Configuration));
                    GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                        new DefaultWebApiHttpHttpControllerActivator(GlobalConfiguration.Configuration));
                    GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

                    ViewEngines.Engines.Clear();
                    ViewEngines.Engines.Add(new ThemeAwareViewEngineShim());
                });

            return webBuilder;
        }
    }
}