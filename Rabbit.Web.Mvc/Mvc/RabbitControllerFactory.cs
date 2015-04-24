using Autofac;
using Autofac.Core;
using Autofac.Features.Metadata;
using Rabbit.Kernel.Works;
using Rabbit.Web.Works;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.Mvc
{
    internal sealed class RabbitControllerFactory : DefaultControllerFactory
    {
        #region Overrides of DefaultControllerFactory

        /// <summary>
        /// 检索指定名称和请求上下文的控制器类型。
        /// </summary>
        /// <returns>
        /// 控制器类型。
        /// </returns>
        /// <param name="requestContext">HTTP 请求的上下文，其中包括 HTTP 上下文和路由数据。</param><param name="controllerName">控制器的名称。</param>
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            var routeData = requestContext.RouteData;

            //确定请求的区域名称，然后回落到具体的控制器。
            var areaName = routeData.GetAreaName();

            //服务名称模式匹配的识别方法
            var serviceKey = (areaName + "/" + controllerName).ToLowerInvariant();

            //现在，要求容器被称为 - 尝试解决控制器的信息
            Meta<Lazy<IController>> info;
            var workContext = requestContext.GetWorkContext();
            if (TryResolve(workContext, serviceKey, out info))
            {
                return (Type)info.Metadata["ControllerType"];
            }

            return null;
        }

        /// <summary>
        /// 检索指定请求上下文和控制器类型的控制器实例。
        /// </summary>
        /// <returns>
        /// 控制器实例。
        /// </returns>
        /// <param name="requestContext">HTTP 请求的上下文，其中包括 HTTP 上下文和路由数据。</param><param name="controllerType">控制器的类型。</param><exception cref="T:System.Web.HttpException"><paramref name="controllerType"/> 为 null。</exception><exception cref="T:System.ArgumentException">无法分配 <paramref name="controllerType"/>。</exception><exception cref="T:System.InvalidOperationException">无法创建 <paramref name="controllerType"/> 的实例。</exception>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController controller;
            var workContext = requestContext.GetWorkContext();
            return TryResolve(workContext, controllerType, out controller) ? controller : base.GetControllerInstance(requestContext, controllerType);
        }

        #endregion Overrides of DefaultControllerFactory

        #region Private Method

        private static bool TryResolve<T>(WorkContext workContext, object serviceKey, out T instance)
        {
            if (workContext != null && serviceKey != null)
            {
                var key = new KeyedService(serviceKey, typeof(T));
                object value;
                if (workContext.Resolve<ILifetimeScope>().TryResolveService(key, out value))
                {
                    instance = (T)value;
                    return true;
                }
            }

            instance = default(T);
            return false;
        }

        #endregion Private Method
    }
}