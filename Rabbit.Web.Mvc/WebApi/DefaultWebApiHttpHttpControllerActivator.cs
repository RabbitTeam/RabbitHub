using Autofac;
using Autofac.Core;
using Autofac.Features.Metadata;
using Rabbit.Kernel.Works;
using Rabbit.Web.Mvc.WebApi.Extensions;
using Rabbit.Web.Mvc.Works;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Rabbit.Web.Mvc.WebApi
{
    internal sealed class DefaultWebApiHttpHttpControllerActivator : IHttpControllerActivator
    {
        #region Field

        private readonly HttpConfiguration _configuration;

        #endregion Field

        #region Constructor

        public DefaultWebApiHttpHttpControllerActivator(HttpConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion Constructor

        #region Implementation of IHttpControllerActivator

        /// <summary>
        /// 创建一个 <see cref="T:System.Web.Http.Controllers.IHttpController"/> 对象。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Web.Http.Controllers.IHttpController"/> 对象。
        /// </returns>
        /// <param name="request">消息请求。</param><param name="controllerDescriptor">HTTP 控制器描述符。</param><param name="controllerType">控制器的类型。</param>
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var routeData = request.GetRouteData();

            var controllerContext = new HttpControllerContext(_configuration, routeData, request);

            var areaName = routeData.GetAreaName();

            var serviceKey = (areaName + "/" + controllerDescriptor.ControllerName).ToLowerInvariant();

            Meta<Lazy<IHttpController>> info;
            var workContext = controllerContext.GetWorkContext();
            if (!TryResolve(workContext, serviceKey, out info))
                return null;
            controllerContext.ControllerDescriptor =
                new HttpControllerDescriptor(_configuration, controllerDescriptor.ControllerName, controllerType);

            var controller = info.Value.Value;

            controllerContext.Controller = controller;

            return controller;
        }

        #endregion Implementation of IHttpControllerActivator

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