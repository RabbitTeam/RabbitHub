using Autofac;
using Rabbit.Kernel;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.Descriptor;
using Rabbit.Kernel.Events;
using Rabbit.Web.Environment;
using Rabbit.Web.Environment.Impl;
using Rabbit.Web.Routes;
using Rabbit.Web.Routes.Impl;
using System;
using System.Web.Routing;

namespace Rabbit.Web
{
    /// <summary>
    ///     建设者扩展方法。
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        ///     使用Web。
        /// </summary>
        /// <param name="kernelBuilder">内核建设者。</param>
        /// <param name="webBuilder">Web建设动作。</param>
        /// <param name="routes">路由集合，如果为 null 则默认为 RouteTable.Routes。</param>
        public static void UseWeb(this IKernelBuilder kernelBuilder, Action<IWebBuilder> webBuilder, RouteCollection routes = null)
        {
            kernelBuilder
                .RegisterExtension(typeof(BuilderExtensions).Assembly)
                .OnStarting(builder =>
                {
                    builder.RegisterInstance(routes ?? RouteTable.Routes);

                    builder.RegisterType<WebHost>().As<IWebHost>().As<IHost>().As<IEventHandler>()
                        /*.Named<IEventHandler>(typeof(IShellSettingsManagerEventHandler).Name)
                    .Named<IEventHandler>(typeof(IShellDescriptorManagerEventHandler).Name)*/
                        .As<IShellSettingsManagerEventHandler>()
                        .As<IShellDescriptorManagerEventHandler>()
                        .SingleInstance();

                    builder.RegisterModule<WebModule>();
                    builder.RegisterType<DefaultRunningShellTable>().As<IRunningShellTable>().SingleInstance();
                });

            if (webBuilder != null)
                webBuilder(new WebBuilder(kernelBuilder));
        }

        /// <summary>
        ///     一个抽象的Web建设者。
        /// </summary>
        public interface IWebBuilder
        {
            /// <summary>
            ///     内核建设者。
            /// </summary>
            IKernelBuilder KernelBuilder { get; }
        }

        private class WebBuilder : IWebBuilder
        {
            public WebBuilder(IKernelBuilder kernelBuilder)
            {
                KernelBuilder = kernelBuilder;
            }

            #region Implementation of IWebBuilder

            public IKernelBuilder KernelBuilder { get; private set; }

            #endregion Implementation of IWebBuilder
        }
    }
}