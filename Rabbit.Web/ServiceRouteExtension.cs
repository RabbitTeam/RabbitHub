using Rabbit.Web.Routes;
using Rabbit.Web.Wcf;
using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;

namespace Rabbit.Web
{
    /// <summary>
    /// 服务路由扩展方法。
    /// </summary>
    public static class ServiceRouteExtension
    {
        /// <summary>
        /// 映射一个Rabbit服务路由。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="routePrefix">路由前缀。</param>
        /// <returns>对映射路由的引用。</returns>
        public static RouteDescriptor MapRabbitServiceRoute<TService>(this ICollection<RouteDescriptor> routes, string routePrefix)
        {
            return routes.MapRabbitServiceRoute(null, routePrefix, typeof(TService));
        }

        /// <summary>
        /// 映射一个Rabbit服务路由。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="name">要映射的路由的名称。</param>
        /// <param name="routePrefix">路由前缀。</param>
        /// <returns>对映射路由的引用。</returns>
        public static RouteDescriptor MapRabbitServiceRoute<TService>(this ICollection<RouteDescriptor> routes, string name, string routePrefix)
        {
            return routes.MapRabbitServiceRoute(name, 0, routePrefix, typeof(TService));
        }

        /// <summary>
        /// 映射一个Rabbit服务路由。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="name">要映射的路由的名称。</param>
        /// <param name="priority">优先级。</param>
        /// <param name="routePrefix">路由前缀。</param>
        /// <returns>对映射路由的引用。</returns>
        public static RouteDescriptor MapRabbitServiceRoute<TService>(this ICollection<RouteDescriptor> routes, string name, int priority, string routePrefix)
        {
            return routes.MapRabbitServiceRoute(name, priority, routePrefix, typeof(TService));
        }

        /// <summary>
        /// 映射一个Rabbit服务路由。
        /// </summary>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="routePrefix">路由前缀。</param>
        /// <param name="serviceType">服务类型。</param>
        /// <returns>对映射路由的引用。</returns>
        public static RouteDescriptor MapRabbitServiceRoute(this ICollection<RouteDescriptor> routes, string routePrefix, Type serviceType)
        {
            return routes.MapRabbitServiceRoute(null, routePrefix, serviceType);
        }

        /// <summary>
        /// 映射一个Rabbit服务路由。
        /// </summary>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="name">要映射的路由的名称。</param>
        /// <param name="routePrefix">路由前缀。</param>
        /// <param name="serviceType">服务类型。</param>
        /// <returns>对映射路由的引用。</returns>
        public static RouteDescriptor MapRabbitServiceRoute(this ICollection<RouteDescriptor> routes, string name, string routePrefix, Type serviceType)
        {
            return routes.MapRabbitServiceRoute(name, 0, routePrefix, serviceType);
        }

        /// <summary>
        /// 映射一个Rabbit服务路由。
        /// </summary>
        /// <param name="routes">应用程序的路由的集合。</param>
        /// <param name="name">要映射的路由的名称。</param>
        /// <param name="priority">优先级。</param>
        /// <param name="routePrefix">路由前缀。</param>
        /// <param name="serviceType">服务类型。</param>
        /// <returns>对映射路由的引用。</returns>
        public static RouteDescriptor MapRabbitServiceRoute(this ICollection<RouteDescriptor> routes, string name, int priority, string routePrefix, Type serviceType)
        {
            var route = new RouteDescriptor
            {
                Name = name,
                Priority = priority,
                Route = new ServiceRoute(routePrefix, new RabbitServiceHostFactory(), serviceType)
            };
            routes.Add(route);
            return route;
        }
    }
}