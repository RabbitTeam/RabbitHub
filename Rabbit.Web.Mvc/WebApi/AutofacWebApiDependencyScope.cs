using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace Rabbit.Web.Mvc.WebApi
{
    internal sealed class AutofacWebApiDependencyScope : IDependencyScope
    {
        #region Field

        private readonly ILifetimeScope _lifetimeScope;

        #endregion Field

        #region Constructor

        public AutofacWebApiDependencyScope(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        #endregion Constructor

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            if (_lifetimeScope != null)
                _lifetimeScope.Dispose();
        }

        #endregion Implementation of IDisposable

        #region Implementation of IDependencyScope

        /// <summary>
        /// 从范围中检索服务。
        /// </summary>
        /// <returns>
        /// 检索到的服务。
        /// </returns>
        /// <param name="serviceType">要检索的服务。</param>
        public object GetService(Type serviceType)
        {
            return _lifetimeScope.ResolveOptional(serviceType);
        }

        /// <summary>
        /// 从范围中检索服务集合。
        /// </summary>
        /// <returns>
        /// 检索到的服务集合。
        /// </returns>
        /// <param name="serviceType">要检索的服务集合。</param>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (!_lifetimeScope.IsRegistered(serviceType))
                return Enumerable.Empty<object>();

            var enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            var instance = _lifetimeScope.Resolve(enumerableServiceType);
            return (IEnumerable<object>)instance;
        }

        #endregion Implementation of IDependencyScope
    }
}