using Rabbit.Kernel;
using System.Collections.Generic;

namespace Rabbit.Web.Routes
{
    /// <summary>
    /// 一个抽象的路由提供程序。
    /// </summary>
    public interface IRouteProvider : IDependency
    {
        /// <summary>
        /// 获取路由信息。
        /// </summary>
        /// <param name="routes">路由集合。</param>
        void GetRoutes(ICollection<RouteDescriptor> routes);
    }
}