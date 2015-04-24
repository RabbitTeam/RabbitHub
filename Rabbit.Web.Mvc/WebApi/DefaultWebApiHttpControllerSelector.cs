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
    internal sealed class DefaultWebApiHttpControllerSelector : DefaultHttpControllerSelector
    {
        #region Field

        private readonly HttpConfiguration _configuration;

        #endregion Field

        #region Constructor

        public DefaultWebApiHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        #endregion Constructor

        #region Overrides of DefaultHttpControllerSelector

        /// <summary>
        /// 为给定 <see cref="T:System.Net.Http.HttpRequestMessage"/> 选择 <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor"/>。
        /// </summary>
        /// <returns>
        /// 给定 <see cref="T:System.Net.Http.HttpRequestMessage"/> 的 <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor"/> 实例。
        /// </returns>
        /// <param name="request">HTTP 请求消息。</param>
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();

            var areaName = routeData.GetAreaName();

            var controllerName = GetControllerName(request);

            //服务名称模式匹配的识别方法
            var serviceKey = (areaName + "/" + controllerName).ToLowerInvariant();

            var controllerContext = new HttpControllerContext(_configuration, routeData, request);

            Meta<Lazy<IHttpController>> info;
            var workContext = controllerContext.GetWorkContext();
            if (!TryResolve(workContext, serviceKey, out info))
                return null;
            var type = (Type)info.Metadata["ControllerType"];

            return
                new HttpControllerDescriptor(_configuration, controllerName, type);
        }

        #endregion Overrides of DefaultHttpControllerSelector

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