using Autofac.Core;
using Rabbit.Kernel.Works;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Rabbit.Web.Wcf
{
    internal sealed class RabbitInstanceProvider : IInstanceProvider
    {
        #region Field

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IComponentRegistration _componentRegistration;

        #endregion Field

        #region Constructor

        public RabbitInstanceProvider(IWorkContextAccessor workContextAccessor, IComponentRegistration componentRegistration)
        {
            _workContextAccessor = workContextAccessor;
            _componentRegistration = componentRegistration;
        }

        #endregion Constructor

        #region Implementation of IInstanceProvider

        /// <summary>
        /// 如果给出指定的 <see cref="T:System.ServiceModel.InstanceContext"/> 对象，则返回服务对象。
        /// </summary>
        /// <returns>
        /// 用户定义的服务对象。
        /// </returns>
        /// <param name="instanceContext">当前的 <see cref="T:System.ServiceModel.InstanceContext"/> 对象。</param>
        public object GetInstance(InstanceContext instanceContext)
        {
            var item = new RabbitInstanceContext(_workContextAccessor);
            instanceContext.Extensions.Add(item);
            return item.Resolve(_componentRegistration);
        }

        /// <summary>
        /// 如果给出指定的 <see cref="T:System.ServiceModel.InstanceContext"/> 对象，则返回服务对象。
        /// </summary>
        /// <returns>
        /// 服务对象。
        /// </returns>
        /// <param name="instanceContext">当前的 <see cref="T:System.ServiceModel.InstanceContext"/> 对象。</param><param name="message">触发服务对象的创建的消息。</param>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return GetInstance(instanceContext);
        }

        /// <summary>
        /// 在 <see cref="T:System.ServiceModel.InstanceContext"/> 对象回收服务对象时调用。
        /// </summary>
        /// <param name="instanceContext">服务的实例上下文。</param><param name="instance">要回收的服务对象。</param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            var context = instanceContext.Extensions.Find<RabbitInstanceContext>();
            if (context != null)
                context.Dispose();
        }

        #endregion Implementation of IInstanceProvider
    }
}