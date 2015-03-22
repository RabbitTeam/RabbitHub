using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rabbit.Kernel.Caching.Impl
{
    internal sealed class DefaultAsyncTokenProvider : IAsyncTokenProvider
    {
        #region Constructor

        public DefaultAsyncTokenProvider()
        {
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IAsyncTokenProvider

        /// <summary>
        /// 获取象征。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <returns>挥发象征。</returns>
        public IVolatileToken GetToken(Action<Action<IVolatileToken>> task)
        {
            var token = new AsyncVolativeToken(task, Logger);
            token.QueueWorkItem();
            return token;
        }

        #endregion Implementation of IAsyncTokenProvider

        #region Help Class

        private sealed class AsyncVolativeToken : IVolatileToken
        {
            private readonly Action<Action<IVolatileToken>> _task;
            private readonly List<IVolatileToken> _taskTokens = new List<IVolatileToken>();
            private volatile Exception _taskException;
            private volatile bool _isTaskFinished;

            public AsyncVolativeToken(Action<Action<IVolatileToken>> task, ILogger logger)
            {
                _task = task;
                Logger = logger;
            }

            private ILogger Logger { get; set; }

            public void QueueWorkItem()
            {
                //启动一个工作项
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        _task(token => _taskTokens.Add(token));
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "发生了错误。");
                        _taskException = e;
                    }
                    finally
                    {
                        _isTaskFinished = true;
                    }
                });
            }

            public bool IsCurrent
            {
                get
                {
                    if (_taskException != null)
                        return false;
                    return !_isTaskFinished || _taskTokens.All(t => t.IsCurrent);
                }
            }
        }

        #endregion Help Class
    }
}