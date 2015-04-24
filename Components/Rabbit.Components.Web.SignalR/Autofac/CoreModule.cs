using Autofac;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Rabbit.Components.Web.SignalR.Autofac
{
    internal sealed class CoreModule : Module
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
            builder
                .RegisterType<AutofacDependencyResolver>()
                .As<IDependencyResolver>()
                .InstancePerDependency();

            builder.RegisterSource(new HubsRegistrationSource());
            builder.RegisterSource(new PersistentConnectionRegistrationSource());

            builder.RegisterType<DefaultHubDescriptorProvider>()
                .As<IHubDescriptorProvider>()
                .InstancePerLifetimeScope()
                .InstancePerMatchingLifetimeScope("shell");

            builder.RegisterType<NullAssemblyLocator>()
                .As<IAssemblyLocator>()
                .InstancePerLifetimeScope()
                .InstancePerMatchingLifetimeScope("shell");
        }

        #endregion Overrides of Module
    }
}