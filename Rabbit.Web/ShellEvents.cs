using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using Rabbit.Web.Routes;
using System.Collections.Generic;

namespace Rabbit.Web
{
    internal sealed class ShellEvents : IShellEvents
    {
        private readonly IEnumerable<IRouteProvider> _routeProviders;
        private readonly IRoutePublisher _routePublisher;

        public ShellEvents(IEnumerable<IRouteProvider> routeProviders, IRoutePublisher routePublisher)
        {
            _routeProviders = routeProviders;
            _routePublisher = routePublisher;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        #region Implementation of IShellEvents

        /// <summary>
        /// 激活外壳完成后执行。
        /// </summary>
        public void Activated()
        {
            var allRoutes = new List<RouteDescriptor>();

            _routeProviders.Invoke(i => i.GetRoutes(allRoutes), Logger);

            _routePublisher.Publish(allRoutes);
        }

        /// <summary>
        /// 终止外壳前候执行。
        /// </summary>
        public void Terminating()
        {
        }

        #endregion Implementation of IShellEvents
    }
}