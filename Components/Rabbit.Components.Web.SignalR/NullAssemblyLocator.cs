using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Reflection;

namespace Rabbit.Components.Web.SignalR
{
    internal sealed class NullAssemblyLocator : IAssemblyLocator
    {
        #region Implementation of IAssemblyLocator

        public IList<Assembly> GetAssemblies()
        {
            return new Assembly[0];
        }

        #endregion Implementation of IAssemblyLocator
    }
}