using Rabbit.Web.Routes;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web
{
    /// <summary>
    /// 路由扩展方法。
    /// </summary>
    public static class RouteExtension
    {
        #region Public Method

        /// <summary>
        /// 获取区域名称。
        /// </summary>
        /// <param name="route">路由信息。</param>
        /// <returns>区域名称。</returns>
        public static string GetAreaName(this RouteBase route)
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
        public static string GetAreaName(this RouteData routeData)
        {
            object area;
            if (routeData.DataTokens.TryGetValue("area", out area))
            {
                return area as string;
            }

            return GetAreaName(routeData.Route);
        }

        /// <summary>
        /// 映射指定的 URL 路由同时指定 <typeparamref name="TController"/> 类型的程序集短名称作为区域并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <typeparam name="TController">控制器类型。</typeparam>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="action">动作。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="routes"/> 或 <paramref name="url"/> 参数为 null。</exception>
        public static RouteDescriptor MapRabbitRoute<TController>(this ICollection<RouteDescriptor> routes, string url, string action, object defaults = null, object constraints = null, object dataTokens = null) where TController : IController
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            return routes.MapRabbitRoute<TController>(null, url, action, defaults, constraints, dataTokens);
        }

        /// <summary>
        /// 映射指定的 URL 路由同时指定 <typeparamref name="TController"/> 类型的程序集短名称作为区域并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <typeparam name="TController">控制器类型。</typeparam>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="name">要映射的路由的名称。</param>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="action">动作。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="routes"/> 或 <paramref name="url"/> 参数为 null。</exception>
        public static RouteDescriptor MapRabbitRoute<TController>(this ICollection<RouteDescriptor> routes, string name, string url, string action, object defaults = null, object constraints = null, object dataTokens = null) where TController : IController
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            var controllerType = typeof(TController);
            var assembly = controllerType.Assembly;

            if (assembly == null)
                throw new ArgumentException("无法中泛型参数中获取程序集信息，所以无法找到区域名称。");

            var areaName = assembly.GetName().Name;
            var controllerName = controllerType.Name;
            if (controllerName.EndsWith("Controller"))
                controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);

            return routes.MapRabbitRoute(name, url, areaName, controllerName, action, defaults, constraints, dataTokens);
        }

        /// <summary>
        /// 映射指定的 URL 路由并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="area">区域。</param>
        /// <param name="controller">控制器名称。</param>
        /// <param name="action">动作。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="routes"/> 或 <paramref name="url"/> 参数为 null。</exception>
        public static RouteDescriptor MapRabbitRoute(this ICollection<RouteDescriptor> routes, string url, string area, string controller, string action = null, object defaults = null, object constraints = null, object dataTokens = null)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            return routes.MapRabbitRoute(null, url, area, controller, action, defaults, constraints, dataTokens);
        }

        /// <summary>
        /// 映射指定的 URL 路由并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="name">要映射的路由的名称。</param>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="area">区域。</param>
        /// <param name="controller">控制器名称。</param>
        /// <param name="action">动作。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="routes"/> 或 <paramref name="url"/> 参数为 null。</exception>
        public static RouteDescriptor MapRabbitRoute(this ICollection<RouteDescriptor> routes, string name, string url, string area, string controller, string action, object defaults = null, object constraints = null,
            object dataTokens = null)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            var defaultsDictionary = new RouteValueDictionary { { "area", area }, { "controller", controller }, { "action", action } };
            var dataTokensDictionary = new RouteValueDictionary { { "area", area } };

            foreach (var item in CreateRouteValueDictionary(defaults))
                defaultsDictionary[item.Key] = item.Value;
            foreach (var item in CreateRouteValueDictionary(dataTokens))
                dataTokensDictionary[item.Key] = item.Value;

            var route = new RouteDescriptor
            {
                Name = name,
                Route = CreateRoute(url, defaultsDictionary, CreateRouteValueDictionary(constraints), dataTokensDictionary)
            };
            routes.Add(route);
            return route;
        }

        /// <summary>
        /// 映射指定的 URL 路由并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="area">区域。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="routes"/> 或 <paramref name="url"/> 参数为 null。</exception>
        public static RouteDescriptor MapRabbitRoute(this ICollection<RouteDescriptor> routes, string url, string area, object defaults = null, object constraints = null, object dataTokens = null)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            return routes.MapRabbitRoute(null, url, area, defaults, constraints, dataTokens);
        }

        /// <summary>
        /// 映射指定的 URL 路由并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="name">要映射的路由的名称。</param>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="area">区域。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="routes"/> 或 <paramref name="url"/> 参数为 null。</exception>
        public static RouteDescriptor MapRabbitRoute(this ICollection<RouteDescriptor> routes, string name, string url, string area, object defaults = null, object constraints = null, object dataTokens = null)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            var defaultsDictionary = new RouteValueDictionary { { "area", area } };
            var dataTokensDictionary = new RouteValueDictionary { { "area", area } };

            foreach (var item in CreateRouteValueDictionary(defaults))
                defaultsDictionary[item.Key] = item.Value;
            foreach (var item in CreateRouteValueDictionary(dataTokens))
                dataTokensDictionary[item.Key] = item.Value;

            var route = new RouteDescriptor
            {
                Name = name,
                Route = CreateRoute(url, defaultsDictionary, CreateRouteValueDictionary(constraints), dataTokensDictionary)
            };
            routes.Add(route);
            return route;
        }

        /// <summary>
        /// 映射指定的 URL 路由并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="routes"/> 或 <paramref name="url"/> 参数为 null。</exception>
        public static Route MapRoute(this RouteCollection routes, string url, object defaults, object constraints, object dataTokens)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            return routes.MapRoute(null, url, defaults, constraints, dataTokens);
        }

        /// <summary>
        /// 映射指定的 URL 路由并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="name">要映射的路由的名称。</param>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="routes"/> 或 <paramref name="url"/> 参数为 null。</exception>
        public static Route MapRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, object dataTokens)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            var route = CreateRoute(url, defaults, constraints, dataTokens);

            if (string.IsNullOrWhiteSpace(name))
                routes.Add(route);
            else
                routes.Add(name, route);

            return route;
        }

        #endregion Public Method

        #region Private Method

        /// <summary>
        /// 映射指定的 URL 路由并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> 参数为 null。</exception>
        private static Route CreateRoute(string url, object defaults,
           object constraints, object dataTokens)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return CreateRoute(url, CreateRouteValueDictionary(defaults), CreateRouteValueDictionary(constraints),
                CreateRouteValueDictionary(dataTokens));
        }

        /// <summary>
        /// 映射指定的 URL 路由并设置默认的路由值、约束和命名空间。
        /// </summary>
        /// <param name="url">路由的 URL 模式。</param>
        /// <param name="defaults">一个包含默认路由值的对象。</param>
        /// <param name="constraints">一组表达式，用于指定 url 参数的值。。</param>
        /// <param name="dataTokens">传递到路由处理程序但未用于确定该路由是否匹配特定 URL 模式的自定义值。这些值会传递到路由处理程序，以便用于处理请求。</param>
        /// <returns>对映射路由的引用。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> 参数为 null。</exception>
        private static Route CreateRoute(string url, RouteValueDictionary defaults,
           RouteValueDictionary constraints, RouteValueDictionary dataTokens)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return new Route(url, defaults, constraints, dataTokens, new MvcRouteHandler());
        }

        private static RouteValueDictionary CreateRouteValueDictionary(object values)
        {
            var dictionary = values as IDictionary<string, object>;
            return dictionary != null ? new RouteValueDictionary(dictionary) : new RouteValueDictionary(values);
        }

        #endregion Private Method
    }
}