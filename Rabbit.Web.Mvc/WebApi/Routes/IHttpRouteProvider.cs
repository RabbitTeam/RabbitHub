using Rabbit.Kernel;
using Rabbit.Web.Routes;
using System.Collections.Generic;

namespace Rabbit.Web.Mvc.WebApi.Routes
{
    /// <summary>
    /// 一个抽象的WebApi路由器提供程序。
    /// </summary>
    public interface IHttpRouteProvider : IDependency
    {
        /// <summary>
        /// 获取路由信息。
        /// </summary>
        /// <param name="routes">路由集合。</param>
        void GetRoutes(ICollection<RouteDescriptor> routes);
    }
}