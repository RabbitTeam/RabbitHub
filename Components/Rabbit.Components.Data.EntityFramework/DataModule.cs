using Autofac;
using Rabbit.Components.Data.EntityFramework.Impl;

namespace Rabbit.Components.Data.EntityFramework
{
    internal sealed class DataModule : Module
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
            if (GlobalConfig.AutomaticMigrationsEnabled)
            {
                builder.RegisterType<AutomaticMigrationsDbContextFactoryEventHandler>()
                    .As<IDbContextFactoryEventHandler>()
                    .SingleInstance();
            }
            builder.RegisterType<EntityFrameworkDbContextFactory>().AsSelf().SingleInstance();
            builder.RegisterGeneric(typeof(EntityFrameworkRepository<>))
                .As(typeof(IRepository<>))
                .InstancePerDependency();
        }

        #endregion Overrides of Module
    }
}