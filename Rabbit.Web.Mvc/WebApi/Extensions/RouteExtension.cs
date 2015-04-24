using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.WebApi.Extensions
{
    /// <summary>
    /// 路由扩展方法。
    /// </summary>
    public static class RouteExtension
    {
        /// <summary>
        /// 获取区域名称。
        /// </summary>
        /// <param name="route">路由信息。</param>
        /// <returns>区域名称。</returns>
        public static string GetAreaName(this IHttpRoute route)
        {
            var routeWithArea = route as IRouteWithArea;
            if (routeWithArea != null)
            {
                return routeWithArea.Area;
            }

            var castRoute = route as Route;
            if (castRoute != null && castRoute.DataTokens != null)
            {
                return castRoute.DataTokens["area"] as string;
            }

            return null;
        }

        /// <summary>
        /// 获取区域名称。
        /// </summary>
        /// <param name="routeData">路由数据。</param>
        /// <returns>区域名称。</returns>
        public static string GetAreaName(this IHttpRouteData routeData)
        {
            object area;
            if (routeData.Route.Defaults.TryGetValue("area", out area))
            {
                return area as string;
            }

            return GetAreaName(routeData.Route);
        }
    }
}