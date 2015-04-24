using Autofac;
using Rabbit.Web.Impl;
using Rabbit.Web.Routes;
using Rabbit.Web.Works;

namespace Rabbit.Web
{
    internal sealed class WebModule : Module
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
            builder.RegisterModule<WebWorkContextModule>();
            builder.RegisterType<DefaultHttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            builder.RegisterType<ShellRoute>().InstancePerDependency();
        }

        #endregion Overrides of Module
    }
}