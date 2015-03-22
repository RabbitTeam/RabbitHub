using Autofac;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Works.Impl
{
    internal sealed class WorkContextImplementation : WorkContext
    {
        #region Field

        private readonly IComponentContext _componentContext;
        private readonly ConcurrentDictionary<string, Func<object>> _stateResolvers = new ConcurrentDictionary<string, Func<object>>();
        private readonly IEnumerable<IWorkContextStateProvider> _workContextStateProviders;

        #endregion Field

        #region Constructor

        public WorkContextImplementation(IComponentContext componentContext)
        {
            _componentContext = componentContext;
            _workContextStateProviders = componentContext.Resolve<IEnumerable<IWorkContextStateProvider>>();
        }

        #endregion Constructor

        #region Overrides of WorkContext

        /// <summary>
        /// 解析一个服务。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>服务实例。</returns>
        public override T Resolve<T>()
        {
            return _componentContext.Resolve<T>();
        }

        /// <summary>
        /// 尝试解析一个服务。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <param name="service">服务实例。</param>
        /// <returns>如果解析成功则返回true，否则返回false。</returns>
        public override bool TryResolve<T>(out T service)
        {
            return _componentContext.TryResolve(out service);
        }

        /// <summary>
        /// 获取一个状态。
        /// </summary>
        /// <typeparam name="T">状态类型。</typeparam>
        /// <param name="name">状态名称。</param>
        /// <returns>状态实例。</returns>
        public override T GetState<T>(string name)
        {
            var resolver = _stateResolvers.GetOrAdd(name, FindResolverForState<T>);
            return (T)resolver();
        }

        /// <summary>
        /// 设置一个状态。
        /// </summary>
        /// <typeparam name="T">状态类型。</typeparam>
        /// <param name="name">状态名称。</param>
        /// <param name="value">状态值。</param>
        public override void SetState<T>(string name, T value)
        {
            _stateResolvers[name] = () => value;
        }

        #endregion Overrides of WorkContext

        #region Private Method

        private Func<object> FindResolverForState<T>(string name)
        {
            var resolver = _workContextStateProviders.Select(wcsp => wcsp.Get<T>(name)).FirstOrDefault(value => !Equals(value, default(T)));

            if (resolver == null)
            {
                return () => default(T);
            }
            return () => resolver(this);
        }

        #endregion Private Method
    }
}