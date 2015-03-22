using Autofac;
using System;

namespace Rabbit.Kernel.Environment.Impl
{
    internal sealed class DefaultHostContainer : IHostContainer
    {
        private readonly ILifetimeScope _container;

        public DefaultHostContainer(ILifetimeScope container)
        {
            _container = container;
        }

        #region Implementation of IHostContainer

        /// <summary>
        /// 解析服务。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>服务实例。</returns>
        public T Resolve<T>()
        {
            return Resolve(typeof(T), default(T));
        }

        #endregion Implementation of IHostContainer

        #region Private Method

        private TService Resolve<TService>(Type serviceType, TService defaultValue = default(TService))
        {
            object value;
            return TryResolve(null, serviceType, out value) ? (TService)value : defaultValue;
        }

        private bool TryResolve(string key, Type serviceType, out object value)
        {
            return TryResolveAtScope(_container, key, serviceType, out value);
        }

        private static bool TryResolveAtScope(IComponentContext scope, string key, Type serviceType, out object value)
        {
            if (scope != null)
                return key == null
                    ? scope.TryResolve(serviceType, out value)
                    : scope.TryResolveKeyed(key, serviceType, out value);
            value = null;
            return false;
        }

        #endregion Private Method
    }
}