using Microsoft.AspNet.SignalR;

namespace Rabbit.Components.Web.SignalR
{
    internal sealed class RabbitHubConfiguration : IRabbitHubConfiguration
    {
        #region Field

        private readonly IDependencyResolver _dependencyResolver;

        #endregion Field

        #region Constructor

        public RabbitHubConfiguration(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        #endregion Constructor

        #region Implementation of IRabbitHubConfiguration

        public HubConfiguration ConnectionConfiguration
        {
            get { return new HubConfiguration { Resolver = _dependencyResolver }; }
        }

        #endregion Implementation of IRabbitHubConfiguration
    }
}