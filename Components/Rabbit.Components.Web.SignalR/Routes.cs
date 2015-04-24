using Microsoft.AspNet.SignalR;
using Owin;
using Rabbit.Web.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.SessionState;

namespace Rabbit.Components.Web.SignalR
{
    internal sealed class Routes : IRouteProvider
    {
        #region Field

        private readonly IRabbitHubConfiguration _rabbitHubConfiguration;
        private readonly ITypeHarvester _typeHarvester;

        #endregion Field

        #region Constructor

        public Routes(IRabbitHubConfiguration rabbitHubConfiguration, ITypeHarvester typeHarvester)
        {
            _rabbitHubConfiguration = rabbitHubConfiguration;
            _typeHarvester = typeHarvester;
        }

        #endregion Constructor

        #region Implementation of IRouteProvider

        /// <summary>
        /// 获取路由信息。
        /// </summary>
        /// <param name="routes">路由集合。</param>
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            #region Hub

            var owinRoute = new RouteCollection().MapOwinPath("signalr.hubs", "/signalr"
                , app => app.MapSignalR(string.Empty, _rabbitHubConfiguration.ConnectionConfiguration));

            routes.Add(new RouteDescriptor
            {
                Name = "Rabbit.Components.Web.SignalR",
                Route = owinRoute,
                SessionState = SessionStateBehavior.Disabled,
                Priority = 5000
            });

            #endregion Hub

            #region PersistentConnection

            foreach (var tuple in _typeHarvester.Get<PersistentConnection>())
            {
                var attrs = tuple.Item1.GetCustomAttributes(typeof(ConnectionAttribute), false);

                var typeName = tuple.Item1.Name.ToLowerInvariant();
                var connectionName = typeName.Contains("connection")
                                            ? typeName.Substring(0, typeName.IndexOf("connection", StringComparison.Ordinal))
                                            : typeName;
                var connectionUrl = connectionName;

                if (attrs.Any())
                {
                    var attrName = ((ConnectionAttribute)attrs[0]).Name;
                    var attrUrl = ((ConnectionAttribute)attrs[0]).Url;

                    connectionName = !string.IsNullOrWhiteSpace(attrName) ? attrName : connectionName;
                    connectionUrl = connectionName;

                    connectionUrl = !string.IsNullOrWhiteSpace(attrUrl) ? attrUrl : connectionUrl;
                }

                var tuple1 = tuple;
                routes.Add(new RouteDescriptor
                {
                    Route = new RouteCollection().MapOwinPath(connectionName, "/" + connectionUrl.TrimStart('/'), map => map.MapSignalR(string.Empty, tuple1.Item1, _rabbitHubConfiguration.ConnectionConfiguration)),
                    SessionState = SessionStateBehavior.Disabled,
                    Priority = int.MaxValue - 1
                });
            }

            #endregion PersistentConnection
        }

        #endregion Implementation of IRouteProvider
    }
}