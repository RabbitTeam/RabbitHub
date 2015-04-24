using Rabbit.Kernel.Environment.ShellBuilders.Models;
using Rabbit.Web.Mvc.Environment.Extensions;
using Rabbit.Web.Routes;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace Rabbit.Web.Mvc.Mvc.Routes.Impl
{
    internal sealed class StandardExtensionRouteProvider : IRouteProvider
    {
        private readonly ShellBlueprint _blueprint;

        public StandardExtensionRouteProvider(ShellBlueprint blueprint)
        {
            _blueprint = blueprint;
        }

        #region Implementation of IRouteProvider

        /// <summary>
        /// 获取路由信息。
        /// </summary>
        /// <param name="routes">路由集合。</param>
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        #endregion Implementation of IRouteProvider

        #region Private Method

        private IEnumerable<RouteDescriptor> GetRoutes()
        {
            var displayPathsPerArea = _blueprint.GetControllers().GroupBy(
                x => x.AreaName,
                x => x.Feature.Descriptor.Extension);

            foreach (var item in displayPathsPerArea)
            {
                var areaName = item.Key;
                var extensionDescriptor = item.Distinct().Single();
                var displayPath = extensionDescriptor.Descriptor.Path;
                var defaultSessionState = SessionStateBehavior.Default;
                //                Enum.TryParse(extensionDescriptor.SessionState, true, out defaultSessionState);
                if (string.IsNullOrWhiteSpace(displayPath) || displayPath == "/" || displayPath == "\\")
                {
                    continue;
                }
                yield return new RouteDescriptor
                {
                    Priority = -10,
                    SessionState = defaultSessionState,
                    Route = new Route(
                        "Admin/" + displayPath + "/{action}/{id}",
                        new RouteValueDictionary {
                            {"area", areaName},
                            {"controller", "admin"},
                            {"action", "index"},
                            {"id", string.Empty}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", areaName}
                        },
                        new MvcRouteHandler())
                };

                yield return new RouteDescriptor
                {
                    Priority = -10,
                    SessionState = defaultSessionState,
                    Route = new Route(
                        displayPath + "/{controller}/{action}/{id}",
                        new RouteValueDictionary {
                            {"area", areaName},
                            {"controller", "home"},
                            {"action", "index"},
                            {"id", string.Empty}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", areaName}
                        },
                        new MvcRouteHandler())
                };
            }
        }

        #endregion Private Method
    }
}