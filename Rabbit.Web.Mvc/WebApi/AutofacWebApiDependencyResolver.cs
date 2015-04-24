using Autofac;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Rabbit.Web.Mvc.WebApi
{
    internal sealed class AutofacWebApiDependencyResolver : IDependencyResolver
    {
        #region Field

        private readonly ILifetimeScope _container;
        private readonly IDependencyScope _rootDependencyScope;

        #endregion Field

        #region Constructor

        public AutofacWebApiDependencyResolver(ILifetimeScope container)
        {
            if (container == null) throw new ArgumentNullException("container");

            _container = container;
            _rootDependencyScope = new AutofacWebApiDependencyScope(container);
        }

        #endregion Constructor

        /*

        public ILifetimeScope Container
        {
            get { return _container; }
        }
*/

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            _rootDependencyScope.Dispose();
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
            return _rootDependencyScope.GetService(serviceType);
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
            return _rootDependencyScope.GetServices(serviceType);
        }

        /// <summary>
        /// 开始解析范围。
        /// </summary>
        /// <returns>
        /// 依赖范围。
        /// </returns>
        public IDependencyScope BeginScope()
        {
            var lifetimeScope = _container.BeginLifetimeScope();
            return new AutofacWebApiDependencyScope(lifetimeScope);
        }

        #endregion Implementation of IDependencyScope
    }
}