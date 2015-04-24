using Autofac.Core;
using Rabbit.Kernel.Works;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Rabbit.Web.Wcf
{
    internal sealed class RabbitDependencyInjectionServiceBehavior : IServiceBehavior
    {
        #region Field

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly Type _implementationType;
        private readonly IComponentRegistration _componentRegistration;

        #endregion Field

        #region Constructor

        public RabbitDependencyInjectionServiceBehavior(IWorkContextAccessor workContextAccessor, Type implementationType, IComponentRegistration componentRegistration)
        {
            if (workContextAccessor == null)
                throw new ArgumentNullException("workContextAccessor");

            if (implementationType == null)
                throw new ArgumentNullException("implementationType");

            if (componentRegistration == null)
                throw new ArgumentNullException("componentRegistration");

            _workContextAccessor = workContextAccessor;
            _implementationType = implementationType;
            _componentRegistration = componentRegistration;
        }

        #endregion Constructor

        #region Implementation of IServiceBehavior

        /// <summary>
        /// 用于检查服务宿主和服务说明，从而确定服务是否可成功运行。
        /// </summary>
        /// <param name="serviceDescription">服务说明。</param><param name="serviceHostBase">当前正在构建的服务宿主。</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// 用于向绑定元素传递自定义数据，以支持协定实现。
        /// </summary>
        /// <param name="serviceDescription">服务的服务说明。</param><param name="serviceHostBase">服务的宿主。</param><param name="endpoints">服务终结点。</param><param name="bindingParameters">绑定元素可访问的自定义对象。</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// 用于更改运行时属性值或插入自定义扩展对象（例如错误处理程序、消息或参数拦截器、安全扩展以及其他自定义扩展对象）。
        /// </summary>
        /// <param name="serviceDescription">服务说明。</param><param name="serviceHostBase">当前正在生成的宿主。</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            if (serviceDescription == null)
                throw new ArgumentNullException("serviceDescription");

            if (serviceHostBase == null)
                throw new ArgumentNullException("serviceHostBase");

            var source = serviceDescription.Endpoints.Where(
                ep => ep.Contract.ContractType.IsAssignableFrom(_implementationType)).Select(ep => ep.Contract.Name).ToArray();

            var provider = new RabbitInstanceProvider(_workContextAccessor, _componentRegistration);
            foreach (var dispatcher2 in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>().SelectMany(dispatcher => dispatcher.Endpoints.Where(dispatcher2 => source.Contains(dispatcher2.ContractName))))
                dispatcher2.DispatchRuntime.InstanceProvider = provider;
        }

        #endregion Implementation of IServiceBehavior
    }
}