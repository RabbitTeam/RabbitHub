using Autofac;
using Autofac.Core;
using Rabbit.Kernel.Works;
using System;
using System.Linq;
using System.ServiceModel;

namespace Rabbit.Web.Wcf
{
    internal sealed class RabbitInstanceContext : IExtension<InstanceContext>, IDisposable
    {
        #region Field

        private readonly IWorkContextScope _workContextScope;
        private readonly WorkContext _workContext;

        #endregion Field

        #region Constructor

        public RabbitInstanceContext(IWorkContextAccessor workContextAccessor)
        {
            _workContext = workContextAccessor.GetContext();

            if (_workContext != null)
                return;
            _workContextScope = workContextAccessor.CreateWorkContextScope();
            _workContext = _workContextScope.WorkContext;
        }

        #endregion Constructor

        #region Implementation of IExtension<InstanceContext>

        /// <summary>
        /// 使扩展对象可以查找它聚合的时间。当扩展添加到 <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> 属性中时调用。
        /// </summary>
        /// <param name="owner">聚合此扩展的可扩展对象。</param>
        public void Attach(InstanceContext owner)
        {
        }

        /// <summary>
        /// 使对象可以查找它不再聚合的时间。当从 <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> 属性中移除扩展时调用。
        /// </summary>
        /// <param name="owner">聚合此扩展的可扩展对象。</param>
        public void Detach(InstanceContext owner)
        {
        }

        #endregion Implementation of IExtension<InstanceContext>

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            if (_workContextScope != null)
                _workContextScope.Dispose();
        }

        #endregion Implementation of IDisposable

        #region Public Method

        public object Resolve(IComponentRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");

            return _workContext.Resolve<ILifetimeScope>().ResolveComponent(registration, Enumerable.Empty<Parameter>());
        }

        #endregion Public Method
    }
}