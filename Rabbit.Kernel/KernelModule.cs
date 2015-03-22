using Autofac;
using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Assemblies;
using Rabbit.Kernel.Environment.Assemblies.Impl;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.Configuration.Impl;
using Rabbit.Kernel.Environment.Descriptor;
using Rabbit.Kernel.Environment.Descriptor.Impl;
using Rabbit.Kernel.Environment.Impl;
using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Environment.ShellBuilders.Impl;
using Rabbit.Kernel.Events;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Folders;
using Rabbit.Kernel.Extensions.Folders.Impl;
using Rabbit.Kernel.Extensions.Impl;
using Rabbit.Kernel.Extensions.Loaders;
using Rabbit.Kernel.Extensions.Loaders.Impl;
using Rabbit.Kernel.FileSystems.AppData;
using Rabbit.Kernel.FileSystems.AppData.Impl;
using Rabbit.Kernel.FileSystems.Application;
using Rabbit.Kernel.FileSystems.Application.Impl;
using Rabbit.Kernel.FileSystems.Dependencies;
using Rabbit.Kernel.FileSystems.Dependencies.Impl;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.FileSystems.VirtualPath.Impl;
using Rabbit.Kernel.Services;
using Rabbit.Kernel.Services.Impl;
using System.IO;
using System.Web.Hosting;

namespace Rabbit.Kernel
{
    /// <summary>
    /// 内核模块。
    /// </summary>
    [SuppressDependency("Rabbit.Kernel.KernelModule")]
    internal sealed class KernelModule : Module
    {
        #region Overrides of Module

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be
        ///             registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new CollectionOrderModule());
            builder.RegisterModule(new CacheModule());

            //HostingEnvironment
            {
                if (!HostingEnvironment.IsHosted)
                {
                    builder.RegisterType<DefaultHostEnvironment>().As<IHostEnvironment>().SingleInstance();
                    builder.RegisterType<DefaultBuildManager>().As<IBuildManager>().SingleInstance();
                }
                else
                {
                    builder.RegisterType<WebHostEnvironment>().As<IHostEnvironment>().SingleInstance();
                    builder.RegisterType<WebBuildManager>().As<IBuildManager>().SingleInstance();
                }
            }
            builder.RegisterType<DefaultHostLocalRestart>().As<IHostLocalRestart>().SingleInstance();

            //Assembly Loader
            {
                builder.RegisterType<DefaultAssemblyLoader>().As<IAssemblyLoader>().SingleInstance();
                builder.RegisterType<AppDomainAssemblyNameResolver>().As<IAssemblyNameResolver>().SingleInstance();
                builder.RegisterType<FrameworkAssemblyNameResolver>().As<IAssemblyNameResolver>().SingleInstance();
                builder.RegisterType<GacAssemblyNameResolver>().As<IAssemblyNameResolver>().SingleInstance();
            }

            //FileSystems
            {
                RegisterVolatileProvider<DefaultVirtualPathProvider, IVirtualPathProvider>(builder);
                RegisterVolatileProvider<DefaultVirtualPathMonitor, IVirtualPathMonitor>(builder);

                RegisterVolatileProvider<DefaultAppDataFolder, IAppDataFolder>(builder);
                RegisterVolatileProvider<DefaultApplicationFolder, IApplicationFolder>(builder);

                /*RegisterVolatileProvider<DefaultExtensionDependenciesManager, IExtensionDependenciesManager>(builder);
                RegisterVolatileProvider<DefaultDependenciesFolder, IDependenciesFolder>(builder);
                RegisterVolatileProvider<DefaultAssemblyProbingFolder, IAssemblyProbingFolder>(builder);*/
            }

            RegisterVolatileProvider<DefaultClock, IClock>(builder);
            RegisterVolatileProvider<DefaultDependenciesFolder, IDependenciesFolder>(builder);
            RegisterVolatileProvider<DefaultExtensionDependenciesManager, IExtensionDependenciesManager>(builder);
            RegisterVolatileProvider<DefaultAssemblyProbingFolder, IAssemblyProbingFolder>(builder);

            builder.RegisterType<DefaultServiceTypeHarvester>().As<IServiceTypeHarvester>().SingleInstance();

            builder.RegisterType<DefaultHost>().As<IHost>().As<IEventHandler>()
                .Named<IEventHandler>(typeof(IShellSettingsManagerEventHandler).Name)
                .Named<IEventHandler>(typeof(IShellDescriptorManagerEventHandler).Name)
                .SingleInstance();
            {
                builder.RegisterType<DefaultShellSettingsManager>().As<IShellSettingsManager>().SingleInstance();
                builder.RegisterType<DefaultShellContextFactory>().As<IShellContextFactory>().SingleInstance();
                {
                    builder.RegisterType<DefaultShellDescriptorCache>().As<IShellDescriptorCache>().SingleInstance();
                    builder.RegisterType<DefaultCompositionStrategy>().As<ICompositionStrategy>().SingleInstance();
                    {
                        builder.RegisterType<DefaultExtensionLoaderCoordinator>()
                            .As<IExtensionLoaderCoordinator>()
                            .SingleInstance();
                        builder.RegisterType<DefaultExtensionMonitoringCoordinator>().As<IExtensionMonitoringCoordinator>().SingleInstance();
                        builder.RegisterType<DefaultExtensionManager>().As<IExtensionManager>().SingleInstance();
                        {
                            builder.RegisterType<DefaultExtensionHarvester>().As<IExtensionHarvester>().SingleInstance();
                            builder.RegisterType<ModuleFolders>().As<IExtensionFolders>().SingleInstance()
                                .WithParameter(new NamedParameter("paths", new[] { "~/Modules" }));

                            builder.RegisterType<ReferencedExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                            builder.RegisterType<PrecompiledExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                        }
                        builder.RegisterType<DefaultShell>().As<IShell>().InstancePerMatchingLifetimeScope("shell");
                    }
                }
                builder.RegisterType<DefaultShellContainerFactory>().As<IShellContainerFactory>().SingleInstance();
                builder.RegisterType<DefaultHostContainer>().As<IHostContainer>().InstancePerDependency();
            }

            builder.RegisterType<KernelMinimumShellDescriptorProvider>()
                .As<IMinimumShellDescriptorProvider>()
                .SingleInstance();

            var optionalComponentsConfig = HostingEnvironment.MapPath("~/Config/HostComponents.config");
            if (File.Exists(optionalComponentsConfig))
                builder.RegisterModule(new HostComponentsConfigModule(optionalComponentsConfig));
        }

        #endregion Overrides of Module

        #region Private Method

        private static void RegisterVolatileProvider<TRegister, TService>(ContainerBuilder builder) where TService : IVolatileProvider
        {
            builder.RegisterType<TRegister>()
                .As<TService>()
                .As<IVolatileProvider>()
                .SingleInstance();
        }

        #endregion Private Method
    }
}