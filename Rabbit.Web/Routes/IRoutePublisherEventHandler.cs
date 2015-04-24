using Rabbit.Kernel;
using System.Collections.Generic;

namespace Rabbit.Web.Routes
{
    /// <summary>
    /// 一个抽象的录音发布者事件处理程序。
    /// </summary>
    public interface IRoutePublisherEventHandler : IDependency
    {
        /// <summary>
        /// 发布前。
        /// </summary>
        /// <param name="routeDescriptors">路由描述符。</param>
        void Publishing(IEnumerable<RouteDescriptor> routeDescriptors);

        /// <summary>
        /// 发布后。
        /// </summary>
        /// <param name="routeDescriptors">路由描述符。</param>
        void Published(IEnumerable<RouteDescriptor> routeDescriptors);
    }
}