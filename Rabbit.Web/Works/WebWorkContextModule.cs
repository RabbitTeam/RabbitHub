using Autofac;
using Rabbit.Kernel.Works;
using Rabbit.Web.Works.Impl;

namespace Rabbit.Web.Works
{
    internal sealed class WebWorkContextModule : Module
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
            builder.RegisterType<WebWorkContextAccessor>()
                .As<IWebWorkContextAccessor>()
                .As<IWorkContextAccessor>()
                .InstancePerMatchingLifetimeScope("shell");
        }

        #endregion Overrides of Module
    }
}