using Autofac;
using Autofac.Core;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Works;
using Rabbit.Web.Routes;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Rabbit.Web.Wcf
{
    /// <summary>
    /// Rabbit服务主机工厂。
    /// </summary>
    public class RabbitServiceHostFactory : ServiceHostFactory, IShim
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的Rabbit服务主机工厂。
        /// </summary>
        public RabbitServiceHostFactory()
        {
            HostContainerRegistry.RegisterShim(this);
        }

        #endregion Constructor

        #region Implementation of IShim

        /// <summary>
        /// 主机容器。
        /// </summary>
        public IHostContainer HostContainer { get; set; }

        #endregion Implementation of IShim

        #region Overrides of ServiceHostFactory

        /// <summary>
        /// 创建具有特定基址的 <see cref="T:System.ServiceModel.ServiceHost"/>，并使用指定数据对其进行初始化。
        /// </summary>
        /// <returns>
        /// 具有特定基址的 <see cref="T:System.ServiceModel.ServiceHost"/>。
        /// </returns>
        /// <param name="constructorString">传递给 <see cref="T:System.ServiceModel.ServiceHostBase"/> 实例的由该工厂构建的初始化数据。</param><param name="baseAddresses">类型为 <see cref="T:System.Uri"/> 且包含所承载服务的基址的 <see cref="T:System.Array"/>。</param><exception cref="T:System.ArgumentNullException"><paramref name="baseAddresses"/> 为 null。</exception><exception cref="T:System.InvalidOperationException">未提供宿主上下文，或者 <paramref name="constructorString"/> 为 null 或为空。</exception>
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            IComponentRegistration registration;
            if (constructorString == null)
            {
                throw new ArgumentNullException("constructorString");
            }

            if (constructorString == string.Empty)
            {
                throw new ArgumentOutOfRangeException("constructorString");
            }

            if (HostContainer == null)
            {
                throw new InvalidOperationException();
            }

            //创建工作上下文。
            var workContextAccessor = GetLifetimeScope(constructorString, baseAddresses).Resolve<IWorkContextAccessor>();
            var workContext = workContextAccessor.GetContext();
            if (workContext == null)
            {
                using (var workContextScope = workContextAccessor.CreateWorkContextScope())
                {
                    var lifetimeScope = workContextScope.Resolve<ILifetimeScope>();
                    registration = GetRegistration(lifetimeScope, constructorString);
                }
            }
            else
            {
                var lifetimeScope = workContext.Resolve<ILifetimeScope>();
                registration = GetRegistration(lifetimeScope, constructorString);
            }

            if (registration == null)
                throw new InvalidOperationException();

            if (!registration.Activator.LimitType.IsClass)
                throw new InvalidOperationException();

            return CreateServiceHost(workContextAccessor, registration, registration.Activator.LimitType, baseAddresses);
        }

        #endregion Overrides of ServiceHostFactory

        #region Protected Method

        /// <summary>
        /// 获Ioc域容器。
        /// </summary>
        /// <param name="constructorString">构造字符串。</param>
        /// <param name="baseAddresses">基地址。</param>
        /// <returns>Ioc容器。</returns>
        protected virtual ILifetimeScope GetLifetimeScope(string constructorString, Uri[] baseAddresses)
        {
            var runningShellTable = HostContainer.Resolve<IRunningShellTable>();
            var shellSettings = runningShellTable.Match(baseAddresses.First().Host, baseAddresses.First().LocalPath);
            var host = HostContainer.Resolve<IHost>();
            var shellContext = host.GetShellContext(shellSettings);
            var workContextAccessor = shellContext.Container;
            return workContextAccessor;
        }

        #endregion Protected Method

        #region Private Method

        private ServiceHost CreateServiceHost(IWorkContextAccessor workContextAccessor, IComponentRegistration registration, Type implementationType, Uri[] baseAddresses)
        {
            var host = CreateServiceHost(implementationType, baseAddresses);

            host.Opening += delegate
            {
                host.Description.Behaviors.Add(new RabbitDependencyInjectionServiceBehavior(workContextAccessor, implementationType, registration));
            };

            return host;
        }

        private static IComponentRegistration GetRegistration(IComponentContext lifetimeScope, string constructorString)
        {
            IComponentRegistration registration;
            if (lifetimeScope.ComponentRegistry.TryGetRegistration(
                new KeyedService(constructorString, typeof(object)), out registration)) return registration;
            var serviceType = Type.GetType(constructorString, false);
            if (serviceType != null)
            {
                lifetimeScope.ComponentRegistry.TryGetRegistration(new TypedService(serviceType), out registration);
            }

            return registration;
        }

        #endregion Private Method
    }
}