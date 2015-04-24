using Autofac.Builder;
using Autofac.Core;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;

namespace Rabbit.Components.Web.SignalR.Autofac
{
    internal sealed class HubsRegistrationSource : IRegistrationSource
    {
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
            var serviceWithType = service as IServiceWithType;
            if (serviceWithType == null)
                yield break;

            var serviceType = serviceWithType.ServiceType;
            if (!typeof(IHub).IsAssignableFrom(serviceType))
                yield break;

            var rb = RegistrationBuilder
                .ForType(serviceType)
                .As(typeof(IHub), serviceType)
                .InstancePerDependency();

            yield return rb.CreateRegistration();
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
    }
}