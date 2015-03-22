using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Caching.Impl
{
    internal sealed class DefaultParallelCacheContext : IParallelCacheContext
    {
        #region Field

        private readonly ICacheContextAccessor _cacheContextAccessor;

        #endregion Field

        #region Constructor

        public DefaultParallelCacheContext(ICacheContextAccessor cacheContextAccessor)
        {
            _cacheContextAccessor = cacheContextAccessor;
        }

        #endregion Constructor

        public bool Disabled { get; set; }

        #region Implementation of IParallelCacheContext

        /// <summary>
        /// 创建一个执行上下文。
        /// </summary>
        /// <typeparam name="T">结果类型。</typeparam>
        /// <param name="function">获取结果的动作。</param>
        /// <returns>任务实例。</returns>
        public ITask<T> CreateContextAwareTask<T>(Func<T> function)
        {
            return new TaskWithAcquireContext<T>(_cacheContextAccessor, function);
        }

        /// <summary>
        /// 以并行的方式运行。
        /// </summary>
        /// <typeparam name="T">源元素类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="source">源集合。</param>
        /// <param name="selector">选择器。</param>
        /// <returns>结果集合。</returns>
        public IEnumerable<TResult> RunInParallel<T, TResult>(IEnumerable<T> source, Func<T, TResult> selector)
        {
            if (Disabled)
                return source.Select(selector);

            //创建任务
            var tasks = source.Select(item => CreateContextAwareTask(() => selector(item))).ToList();

            //并行运行任务并立即返回结果
            var result = tasks
                .AsParallel() //并行执行准备
                .AsOrdered() //排序
                .Select(task => task.Execute()) //准备任务并行运行
                .ToArray(); //立即执行

            //收集令牌
            foreach (var task in tasks)
                task.Finish();

            return result;
        }

        #endregion Implementation of IParallelCacheContext

        #region Help Class

        private sealed class TaskWithAcquireContext<T> : ITask<T>
        {
            #region Field

            private readonly ICacheContextAccessor _cacheContextAccessor;
            private readonly Func<T> _function;
            private IList<IVolatileToken> _tokens;

            #endregion Field

            #region Constructor

            public TaskWithAcquireContext(ICacheContextAccessor cacheContextAccessor, Func<T> function)
            {
                _cacheContextAccessor = cacheContextAccessor;
                _function = function;
            }

            #endregion Constructor

            #region Implementation of IDisposable

            /// <summary>
            /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
            /// </summary>
            public void Dispose()
            {
                Finish();
            }

            #endregion Implementation of IDisposable

            #region Implementation of ITask<out T>

            /// <summary>
            /// 执行任务。
            /// </summary>
            /// <returns>任务结果。</returns>
            public T Execute()
            {
                var parentContext = _cacheContextAccessor.Current;
                try
                {
                    // Push context
                    if (parentContext == null)
                    {
                        _cacheContextAccessor.Current = new SimpleAcquireContext(AddToken);
                    }

                    // Execute lambda
                    return _function();
                }
                finally
                {
                    // Pop context
                    if (parentContext == null)
                    {
                        _cacheContextAccessor.Current = parentContext;
                    }
                }
            }

            /// <summary>
            /// 执行任务过程中收集到的令牌。
            /// </summary>
            public IEnumerable<IVolatileToken> Tokens
            {
                get
                {
                    return _tokens ?? Enumerable.Empty<IVolatileToken>();
                }
            }

            /// <summary>
            /// 完成任务。
            /// </summary>
            public void Finish()
            {
                var tokens = _tokens;
                _tokens = null;
                if (_cacheContextAccessor.Current == null || tokens == null)
                    return;
                foreach (var token in tokens)
                {
                    _cacheContextAccessor.Current.Monitor(token);
                }
            }

            #endregion Implementation of ITask<out T>

            #region Private Method

            private void AddToken(IVolatileToken token)
            {
                if (_tokens == null)
                    _tokens = new List<IVolatileToken>();
                _tokens.Add(token);
            }

            #endregion Private Method
        }

        #endregion Help Class
    }
}