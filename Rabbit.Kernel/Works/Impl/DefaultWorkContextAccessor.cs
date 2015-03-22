using Autofac;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Works.Impl
{
    internal sealed class DefaultWorkContextAccessor : IWorkContextAccessor
    {
        #region Field

        private readonly ILifetimeScope _lifetimeScope;
        private readonly object _workContextKey = new object();

        [ThreadStatic]
        private static ConcurrentDictionary<object, WorkContext> _threadStaticContexts;

        #endregion Field

        #region Constructor

        public DefaultWorkContextAccessor(ILifetimeScope lifetimeScope)
        {
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
            WorkContext workContext;
            return EnsureThreadStaticContexts().TryGetValue(_workContextKey, out workContext) ? workContext : null;
        }

        /// <summary>
        /// 创建一个工作上下文范围。
        /// </summary>
        /// <returns>工作上下文范围。</returns>
        public IWorkContextScope CreateWorkContextScope()
        {
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

        #region Private Method

        private static ConcurrentDictionary<object, WorkContext> EnsureThreadStaticContexts()
        {
            return _threadStaticContexts ?? (_threadStaticContexts = new ConcurrentDictionary<object, WorkContext>());
        }

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

        #endregion Private Method
    }
}