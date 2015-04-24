using Rabbit.Web.Routes;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.WebApi.Routes
{
    internal sealed class WebApiRoutePublisherEventHandler : IRoutePublisherEventHandler
    {
        #region Implementation of IRoutePublisherEventHandler

        /// <summary>
        /// 发布前。
        /// </summary>
        /// <param name="routeDescriptors">路由描述符。</param>
        public void Publishing(IEnumerable<RouteDescriptor> routeDescriptors)
        {
            var preloading = new RouteCollection();
            foreach (var routeDescriptor in routeDescriptors)
            {
                // WebApi 路由注册
                var httpRouteDescriptor = routeDescriptor as HttpRouteDescriptor;
                if (httpRouteDescriptor == null)
                    continue;
                var httpRouteCollection = new RouteCollection();
                httpRouteCollection.MapHttpRoute(httpRouteDescriptor.Name, httpRouteDescriptor.RouteTemplate, httpRouteDescriptor.Defaults, httpRouteDescriptor.Constraints);
                routeDescriptor.Route = httpRouteCollection.First();
                preloading.Add(routeDescriptor.Name, routeDescriptor.Route);
            }
        }

        /// <summary>
        /// 发布后。
        /// </summary>
        /// <param name="routeDescriptors">路由描述符。</param>
        public void Published(IEnumerable<RouteDescriptor> routeDescriptors)
        {
        }

        #endregion Implementation of IRoutePublisherEventHandler
    }
}