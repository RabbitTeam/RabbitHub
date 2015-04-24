using Autofac;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using Rabbit.Kernel.Works;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rabbit.Web.Works.Impl
{
    internal sealed class WebWorkContextAccessor : IWebWorkContextAccessor
    {
        #region Field

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly object _workContextKey = new object();

        [ThreadStatic]
        private static ConcurrentDictionary<object, WorkContext> _threadStaticContexts;

        #endregion Field

        #region Constructor

        public WebWorkContextAccessor(IHttpContextAccessor httpContextAccessor, ILifetimeScope lifetimeScope)
        {
            _httpContextAccessor = httpContextAccessor;
            _lifetimeScope = lifetimeScope;
        }

        #endregion Constructor

        #region Implementation of IWorkContextAccessor

        /// <summary>
        /// 获取工作上下文。
        /// </summary>
        /// <returns>工作上下文。</returns>
        public WorkContext GetContext()
        {
            var httpContext = _httpContextAccessor.Current();
            if (httpContext != null)
                return GetContext(httpContext);

            WorkContext workContext;
            return EnsureThreadStaticContexts().TryGetValue(_workContextKey, out workContext) ? workContext : null;
        }

        /// <summary>
        /// 创建一个工作上下文范围。
        /// </summary>
        /// <returns>工作上下文范围。</returns>
        public IWorkContextScope CreateWorkContextScope()
        {
            var httpContext = _httpContextAccessor.Current();
            if (httpContext != null)
                return CreateWorkContextScope(httpContext);

            var workLifetime = _lifetimeScope.BeginLifetimeScope("work");

            var events = workLifetime.Resolve<IEnumerable<IWorkContextEvents>>().ToArray();
            events.Invoke(e => e.Started(), NullLogger.Instance);

            return new ThreadStaticScopeImplementation(
                events,
                workLifetime,
                EnsureThreadStaticContexts(),
                _workContextKey);
        }

        #endregion Implementation of IWorkContextAccessor

        #region Implementation of IWebWorkContextAccessor

        /// <summary>
        /// 获取工作上下文。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        /// <returns>工作上下文。</returns>
        public WorkContext GetContext(HttpContextBase httpContext)
        {
            return httpContext.Items[_workContextKey] as WorkContext;
        }

        /// <summary>
        /// 创建工作上下文范围。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        /// <returns>工作上下文范围。</returns>
        public IWorkContextScope CreateWorkContextScope(HttpContextBase httpContext)
        {
            var workLifetime = _lifetimeScope.BeginLifetimeScope("work");
            workLifetime.Resolve<WorkContextProperty<HttpContextBase>>().Value = httpContext;

            var events = workLifetime.Resolve<IEnumerable<IWorkContextEvents>>().ToArray();
            events.Invoke(e => e.Started(), NullLogger.Instance);

            return new HttpContextScopeImplementation(
                events,
                workLifetime,
                httpContext,
                _workContextKey);
        }

        #endregion Implementation of IWebWorkContextAccessor

        #region Private Method

        private static ConcurrentDictionary<object, WorkContext> EnsureThreadStaticContexts()
        {
            return _threadStaticContexts ?? (_threadStaticContexts = new ConcurrentDictionary<object, WorkContext>());
        }

        #endregion Private Method

        #region Help Class

        private sealed class ThreadStaticScopeImplementation : IWorkContextScope
        {
            #region Field

            private readonly WorkContext _workContext;
            private readonly Action _disposer;

            #endregion Field

            #region Constructor

            public ThreadStaticScopeImplementation(IEnumerable<IWorkContextEvents> events, ILifetimeScope lifetimeScope, ConcurrentDictionary<object, WorkContext> contexts, object workContextKey)
            {
                _workContext = lifetimeScope.Resolve<WorkContext>();
                contexts.AddOrUpdate(workContextKey, _workContext, (a, b) => _workContext);

                _disposer = () =>
                {
                    events.Invoke(e => e.Finished(), NullLogger.Instance);

                    WorkContext removedContext;
                    contexts.TryRemove(workContextKey, out removedContext);
                    lifetimeScope.Dispose();
                };
            }

            #endregion Constructor

            #region Implementation of IWorkContextScope

            /// <summary>
            /// 工作上下文。
            /// </summary>
            public WorkContext WorkContext
            {
                get { return _workContext; }
            }

            /// <summary>
            /// 解析一个服务。
            /// </summary>
            /// <typeparam name="TService">服务类型。</typeparam>
            /// <returns>服务实例。</returns>
            public TService Resolve<TService>()
            {
                return WorkContext.Resolve<TService>();
            }

            /// <summary>
            /// 尝试解析一个服务。
            /// </summary>
            /// <typeparam name="TService">服务类型。</typeparam>
            /// <param name="service">服务实例。</param>
            /// <returns>如果解析成功则返回true，否则返回false。</returns>
            public bool TryResolve<TService>(out TService service)
            {
                return WorkContext.TryResolve(out service);
            }

            #endregion Implementation of IWorkContextScope

            #region Implementation of IDisposable

            /// <summary>
            /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
            /// </summary>
            void IDisposable.Dispose()
            {
                _disposer();
            }

            #endregion Implementation of IDisposable
        }

        private class HttpContextScopeImplementation : IWorkContextScope
        {
            private readonly WorkContext _workContext;
            private readonly Action _disposer;

            public HttpContextScopeImplementation(IEnumerable<IWorkContextEvents> events, ILifetimeScope lifetimeScope, HttpContextBase httpContext, object workContextKey)
            {
                _workContext = lifetimeScope.Resolve<WorkContext>();
                httpContext.Items[workContextKey] = _workContext;

                _disposer = () =>
                {
                    events.Invoke(e => e.Finished(), NullLogger.Instance);

                    httpContext.Items.Remove(workContextKey);
                    lifetimeScope.Dispose();
                };
            }

            void IDisposable.Dispose()
            {
                _disposer();
            }

            public WorkContext WorkContext
            {
                get { return _workContext; }
            }

            public TService Resolve<TService>()
            {
                return WorkContext.Resolve<TService>();
            }

            public bool TryResolve<TService>(out TService service)
            {
                return WorkContext.TryResolve(out service);
            }
        }

        #endregion Help Class
    }
}