using System;
using System.Threading;
using System.Web;

namespace Rabbit.Web.WarmupStarter
{
    /// <summary>
    /// 热启动器。
    /// </summary>
    /// <typeparam name="T">参数类型。</typeparam>
    public class Starter<T> where T : class
    {
        #region Field

        private readonly Func<HttpApplication, T> _initialization;
        private readonly Action<HttpApplication, T> _beginRequest;
        private readonly Action<HttpApplication, T> _endRequest;
        private readonly object _synLock = new object();

        private volatile T _initializationResult;
        private volatile Exception _error;
        private volatile Exception _previousError;

        #endregion Field

        /// <summary>
        /// 是否启动完成。
        /// </summary>
        public static bool IsStarted { get; private set; }

        #region Constructor

        /// <summary>
        /// 初始化一个新的热启动器。
        /// </summary>
        /// <param name="initialization">初始化操作。</param>
        /// <param name="beginRequest">请求开始时的操作。</param>
        /// <param name="endRequest">请求结束时的操作。</param>
        public Starter(Func<HttpApplication, T> initialization, Action<HttpApplication, T> beginRequest, Action<HttpApplication, T> endRequest)
        {
            _initialization = initialization;
            _beginRequest = beginRequest;
            _endRequest = endRequest;
        }

        #endregion Constructor

        #region Public Method

        /// <summary>
        /// 应用程序启动时执行的动作。
        /// </summary>
        /// <param name="application">一个Http应用程序。</param>
        public void OnApplicationStart(HttpApplication application)
        {
            LaunchStartupThread(application);
        }

        /// <summary>
        /// 请求开始时的操作。
        /// </summary>
        /// <param name="application">一个Http应用程序。</param>
        public void OnBeginRequest(HttpApplication application)
        {
            if (_error != null)
            {
                var restartInitialization = false;

                lock (_synLock)
                {
                    if (_error != null)
                    {
                        _previousError = _error;
                        _error = null;
                        restartInitialization = true;
                    }
                }

                if (restartInitialization)
                {
                    LaunchStartupThread(application);
                }
            }

            if (_previousError != null)
            {
                throw new ApplicationException("应用程序初始化时出错", _previousError);
            }

            if (_initializationResult != null)
            {
                _beginRequest(application, _initializationResult);
            }
        }

        /// <summary>
        /// 请求结束时的操作。
        /// </summary>
        /// <param name="application">一个Http应用程序。</param>
        public void OnEndRequest(HttpApplication application)
        {
            if (_initializationResult != null)
            {
                _endRequest(application, _initializationResult);
            }
        }

        #endregion Public Method

        #region Private Method

        private void LaunchStartupThread(HttpApplication application)
        {
            WarmupHttpModule.SignalWarmupStart();

            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    try
                    {
                        var result = _initialization(application);
                        _initializationResult = result;
                    }
                    catch (Exception e)
                    {
                        lock (_synLock)
                        {
                            _error = e;
                            _previousError = null;
                        }
                    }
                    finally
                    {
                        IsStarted = true;
                        WarmupHttpModule.SignalWarmupDone();
                    }
                });
        }

        #endregion Private Method
    }
}