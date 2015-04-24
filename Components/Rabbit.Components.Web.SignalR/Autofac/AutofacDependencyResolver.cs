using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Web.SignalR.Autofac
{
    internal sealed class AutofacDependencyResolver : DefaultDependencyResolver, IRegistrationSource
    {
        #region Field

        private readonly ILifetimeScope _lifetimeScope;

        #endregion Field

        #region Constructor

        public AutofacDependencyResolver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
            _lifetimeScope.ComponentRegistry.AddRegistrationSource(this);
        }

        #endregion Constructor

        #region Overrides of DefaultDependencyResolver

        public override object GetService(Type serviceType)
        {
            return ResolverSystemService(serviceType) ?? _lifetimeScope.ResolveOptional(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var systemServices = ResolverSystemServices(serviceType) ?? new object[0];
            var enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            var instance = _lifetimeScope.Resolve(enumerableServiceType);
            return ((IEnumerable<object>)instance).Concat(systemServices).ToArray();
        }

        public override void Register(Type serviceType, Func<object> activator)
        {
            if (IsSignalR(serviceType))
                GlobalHost.DependencyResolver.Register(serviceType, activator);
            else
                base.Register(serviceType, activator);
        }

        public override void Register(Type serviceType, IEnumerable<Func<object>> activators)
        {
            if (IsSignalR(serviceType))
                GlobalHost.DependencyResolver.Register(serviceType, activators);
            else
                base.Register(serviceType, activators);
        }

        #endregion Overrides of DefaultDependencyResolver

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
            var typedService = service as TypedService;

            if (typedService == null)
                return Enumerable.Empty<IComponentRegistration>();

            var instances = base.GetServices(typedService.ServiceType);

            if (instances != null)
            {
                return instances
                    .Select(i => RegistrationBuilder.ForDelegate(i.GetType(), (c, p) => i).As(typedService.ServiceType)
                        .InstancePerMatchingLifetimeScope(_lifetimeScope.Tag)
                        .PreserveExistingDefaults()
                        .CreateRegistration());
            }

            return Enumerable.Empty<IComponentRegistration>();
        }

        /// <summary>
        /// Gets whether the registrations provided by this source are 1:1 adapters on top
        ///             of other components (I.e. like Meta, Func or Owned.)
        /// </summary>
        public bool IsAdapterForIndividualComponents
        {
            get { return false; }
        }

        #endregion Implementation of IRegistrationSource

        #region Private Method

        private static bool IsSignalR(Type type)
        {
            return type.Assembly == typeof(IHub).Assembly;
        }

        private static object ResolverSystemService(Type type)
        {
            return IsSignalR(type) ? GlobalHost.DependencyResolver.GetService(type) : null;
        }

        private static IEnumerable<object> ResolverSystemServices(Type type)
        {
            return IsSignalR(type) ? GlobalHost.DependencyResolver.GetServices(type) : null;
        }

        #endregion Private Method
    }
}