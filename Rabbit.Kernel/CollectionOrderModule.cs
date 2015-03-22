using Autofac.Core;
using System;
using System.Collections;

namespace Rabbit.Kernel
{
    internal sealed class CollectionOrderModule : IModule
    {
        #region Implementation of IModule

        /// <summary>
        /// Apply the module to the component registry.
        /// </summary>
        /// <param name="componentRegistry">Component registry to apply configuration to.</param>
        public void Configure(IComponentRegistry componentRegistry)
        {
            componentRegistry.Registered += (sender, registered) =>
            {
                var limitType = registered.ComponentRegistration.Activator.LimitType;
                if (typeof(IEnumerable).IsAssignableFrom(limitType))
                {
                    registered.ComponentRegistration.Activated += (sender2, activated) =>
                    {
                        var array = activated.Instance as Array;
                        if (array != null)
                            Array.Reverse(array);
                    };
                }
            };
        }

        #endregion Implementation of IModule
    }
}