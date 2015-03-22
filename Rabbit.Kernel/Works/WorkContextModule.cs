using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Rabbit.Kernel.Works.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using Module = Autofac.Module;

namespace Rabbit.Kernel.Works
{
    internal sealed class WorkContextModule : Module
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
            //TODO:应该考虑更好的机制，重要。
            /*builder.RegisterType<DefaultWorkContextAccessor>()
                .As<IWorkContextAccessor>()
                .InstancePerMatchingLifetimeScope("shell");*/
            if (!HostingEnvironment.IsHosted)
            {
                builder.RegisterType<DefaultWorkContextAccessor>()
                .As<IWorkContextAccessor>()
                .InstancePerMatchingLifetimeScope("shell");
            }

            builder.Register(ctx => new WorkContextImplementation(ctx.Resolve<IComponentContext>()))
                .As<WorkContext>()
                .InstancePerMatchingLifetimeScope("work");

            builder.RegisterType<WorkContextProperty<HttpContextBase>>()
                .As<WorkContextProperty<HttpContextBase>>()
                .InstancePerMatchingLifetimeScope("work");

            builder.Register(ctx => ctx.Resolve<WorkContextProperty<HttpContextBase>>().Value)
                .As<HttpContextBase>()
                .InstancePerDependency();

            builder.RegisterGeneric(typeof(WorkValues<>))
                .InstancePerMatchingLifetimeScope("work");

            builder.RegisterSource(new WorkRegistrationSource());
        }

        #endregion Overrides of Module

        private class WorkValues<T> where T : class
        {
            public WorkValues(IComponentContext componentContext)
            {
                ComponentContext = componentContext;
                Values = new Dictionary<Work<T>, T>();
            }

            public IComponentContext ComponentContext { get; private set; }

            public IDictionary<Work<T>, T> Values { get; private set; }
        }

        private sealed class WorkRegistrationSource : IRegistrationSource
        {
            #region Field

            private static readonly MethodInfo CreateMetaRegistrationMethod = typeof(WorkRegistrationSource).GetMethod(
                "CreateMetaRegistration", BindingFlags.Static | BindingFlags.NonPublic);

            #endregion Field

            #region Private Method

            private static bool IsClosingTypeOf(Type type, Type openGenericType)
            {
                return type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType;
            }

            private static IComponentRegistration CreateMetaRegistration<T>(Service providedService, IComponentRegistration valueRegistration) where T : class
            {
                var rb = RegistrationBuilder.ForDelegate(
                    (c, p) =>
                    {
                        var workContextAccessor = c.Resolve<IWorkContextAccessor>();
                        return new Work<T>(w =>
                        {
                            var workContext = workContextAccessor.GetContext();
                            if (workContext == null)
                                return default(T);

                            var workValues = workContext.Resolve<WorkValues<T>>();

                            T value;
                            if (!workValues.Values.TryGetValue(w, out value))
                            {
                                value = (T)workValues.ComponentContext.ResolveComponent(valueRegistration, p);
                                workValues.Values[w] = value;
                            }
                            return value;
                        });
                    })
                    .As(providedService)
                    .Targeting(valueRegistration);

                return rb.CreateRegistration();
            }

            #endregion Private Method

            #region Implementation of IRegistrationSource

            /// <summary>
            /// Retrieve registrations for an unregistered service, to be used
            ///             by the container.
            /// </summary>
            /// <param name="service">The service that was requested.</param><param name="registrationAccessor">A function that will return existing registrations for a service.</param>
            /// <returns>
            /// Registrations providing the service.
            /// </returns>
            /// <remarks>
            /// If the source is queried for service s, and it returns a component that implements both s and s', then it
            ///             will not be queried again for either s or s'. This means that if the source can return other implementations
            ///             of s', it should return these, plus the transitive closure of other components implementing their
            ///             additional services, along with the implementation of s. It is not an error to return components
            ///             that do not implement <paramref name="service"/>.
            /// </remarks>
            public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
            {
                var swt = service as IServiceWithType;
                if (swt == null || !IsClosingTypeOf(swt.ServiceType, typeof(Work<>)))
                    return Enumerable.Empty<IComponentRegistration>();

                var valueType = swt.ServiceType.GetGenericArguments()[0];

                var valueService = swt.ChangeType(valueType);

                var registrationCreator = CreateMetaRegistrationMethod.MakeGenericMethod(valueType);

                return registrationAccessor(valueService)
                    .Select(v => registrationCreator.Invoke(null, new object[] { service, v }))
                    .Cast<IComponentRegistration>();
            }

            /// <summary>
            /// Gets whether the registrations provided by this source are 1:1 adapters on top
            ///             of other components (I.e. like Meta, Func or Owned.)
            /// </summary>
            public bool IsAdapterForIndividualComponents
            {
                get { return true; }
            }

            #endregion Implementation of IRegistrationSource
        }
    }
}