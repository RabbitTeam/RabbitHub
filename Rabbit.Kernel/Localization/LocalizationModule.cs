using Autofac;
using Autofac.Core;
using Rabbit.Kernel.Localization.Impl;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Module = Autofac.Module;

namespace Rabbit.Kernel.Localization
{
    internal sealed class LocalizationModule : Module
    {
        #region Field

        private readonly ConcurrentDictionary<string, Localizer> _localizerCache;

        #endregion Field

        #region Constructor

        public LocalizationModule()
        {
            _localizerCache = new ConcurrentDictionary<string, Localizer>();
        }

        #endregion Constructor

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
            builder.RegisterType<Text>().As<IText>().InstancePerDependency();
        }

        /// <summary>
        /// Override to attach module-specific functionality to a
        ///             component registration.
        /// </summary>
        /// <remarks>
        /// This method will be called for all existing <i>and future</i> component
        ///             registrations - ordering is not important.
        /// </remarks>
        /// <param name="componentRegistry">The component registry.</param><param name="registration">The registration to attach functionality to.</param>
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var userProperty = FindUserProperty(registration.Activator.LimitType);

            if (userProperty == null)
                return;

            var scope = registration.Activator.LimitType.FullName;

            registration.Activated += (sender, e) =>
            {
                var localizer = _localizerCache.GetOrAdd(scope, key => LocalizationUtilities.Resolve(e.Context, scope));
                userProperty.SetValue(e.Instance, localizer, null);
            };
        }

        #endregion Overrides of Module

        #region Private Method

        private static PropertyInfo FindUserProperty(IReflect type)
        {
            //寻找类型为 "Localizer" 并且具有set方法的属性。
            return type
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType == typeof(Localizer)) //必须是一个本地化委托
                .Where(x => !x.GetIndexParameters().Any()) //没有索引器
                .FirstOrDefault(x => x.GetAccessors(false).Length != 1 || x.GetAccessors(false)[0].ReturnType == typeof(void)); //必须具有set方法。
        }

        #endregion Private Method
    }
}