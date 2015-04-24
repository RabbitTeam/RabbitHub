using Rabbit.Kernel;
using System.Collections.Generic;

namespace Rabbit.Web.Routes
{
    /// <summary>
    /// 一个抽象的路由发布者。
    /// </summary>
    public interface IRoutePublisher : IDependency
    {
        /// <summary>
        /// 发布路由。
        /// </summary>
        /// <param name="routes">路由集合。</param>
        void Publish(IEnumerable<RouteDescriptor> routes);
    }
}