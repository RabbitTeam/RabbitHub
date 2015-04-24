using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Routes.Impl
{
    internal sealed class DefaultRouteProvider : IRouteProvider
    {
        #region Implementation of IRouteProvider

        /// <summary>
        ///     获取路由信息。
        /// </summary>
        /// <param name="routes">路由集合。</param>
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        #endregion Implementation of IRouteProvider

        #region Private Method

        private static IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Priority = -20,
                    Route = new Route(
                        "{controller}/{action}/{id}",
                        new RouteValueDictionary
                        {
                            {"controller", "home"},
                            {"action", "index"},
                            {"id", string.Empty},
                        },
                        new RouteValueDictionary
                        {
                            {"controller", new HomeOrAccount()}
                        },
                        new MvcRouteHandler())
                }
            };
        }

        #endregion Private Method

        #region Help Class

        private sealed class HomeOrAccount : IRouteConstraint
        {
            #region Implementation of IRouteConstraint

            /// <summary>
            ///     确定 URL 参数是否包含此约束的有效值。
            /// </summary>
            /// <returns>
            ///     如果 URL 参数包含有效值，则为 true；否则为 false。
            /// </returns>
            /// <param name="httpContext">一个对象，封装有关 HTTP 请求的信息。</param>
            /// <param name="route">此约束所属的对象。</param>
            /// <param name="parameterName">正在检查的参数的名称。</param>
            /// <param name="values">一个包含 URL 的参数的对象。</param>
            /// <param name="routeDirection">一个对象，指示在处理传入请求或生成 URL 时，是否正在执行约束检查。</param>
            public bool Match(HttpContextBase httpContext, Route route, string parameterName,
                RouteValueDictionary values, RouteDirection routeDirection)
            {
                object value;
                if (!values.TryGetValue(parameterName, out value)) return false;
                var parameterValue = Convert.ToString(value);
                return string.Equals(parameterValue, "home", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(parameterValue, "account", StringComparison.OrdinalIgnoreCase);
            }

            #endregion Implementation of IRouteConstraint
        }

        #endregion Help Class
    }
}