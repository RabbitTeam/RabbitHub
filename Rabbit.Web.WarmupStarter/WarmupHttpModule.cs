using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace Rabbit.Web.WarmupStarter
{
    /// <summary>
    /// 热启动Http模块。
    /// </summary>
    internal sealed class WarmupHttpModule : IHttpModule
    {
        private HttpApplication _context;
        private static readonly object SynLock = new object();
        private static IList<Action> _awaiting = new List<Action>();

        public void Init(HttpApplication context)
        {
            _context = context;
            context.AddOnBeginRequestAsync(BeginBeginRequest, EndBeginRequest, null);
        }

        public void Dispose()
        {
        }

        private static bool InWarmup()
        {
            lock (SynLock)
            {
                return _awaiting != null;
            }
        }

        public static void SignalWarmupStart()
        {
            lock (SynLock)
            {
                if (_awaiting == null)
                {
                    _awaiting = new List<Action>();
                }
            }
        }

        public static void SignalWarmupDone()
        {
            IList<Action> temp;

            lock (SynLock)
            {
                temp = _awaiting;
                _awaiting = null;
            }

            if (temp == null)
                return;
            foreach (var action in temp)
            {
                action();
            }
        }

        private static void Await(Action action)
        {
            var temp = action;

            lock (SynLock)
            {
                if (_awaiting != null)
                {
                    temp = null;
                    _awaiting.Add(action);
                }
            }

            if (temp != null)
            {
                temp();
            }
        }

        private IAsyncResult BeginBeginRequest(object sender, EventArgs e, AsyncCallback cb, object extradata)
        {
            if (!InWarmup() || WarmupUtility.DoBeginRequest(_context))
            {
                var asyncResult = new DoneAsyncResult(extradata);
                cb(asyncResult);
                return asyncResult;
            }
            else
            {
                var asyncResult = new WarmupAsyncResult(cb, extradata);
                Await(asyncResult.Completed);
                return asyncResult;
            }
        }

        private static void EndBeginRequest(IAsyncResult ar)
        {
        }

        private sealed class WarmupAsyncResult : IAsyncResult, IDisposable
        {
            private readonly EventWaitHandle _eventWaitHandle = new AutoResetEvent(false);
            private readonly AsyncCallback _cb;
            private readonly object _asyncState;
            private bool _isCompleted;

            public WarmupAsyncResult(AsyncCallback cb, object asyncState)
            {
                _cb = cb;
                _asyncState = asyncState;
                _isCompleted = false;
            }

            public void Completed()
            {
                _isCompleted = true;
                _eventWaitHandle.Set();
                _cb(this);
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return false; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return _isCompleted; }
            }

            object IAsyncResult.AsyncState
            {
                get { return _asyncState; }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return _eventWaitHandle; }
            }

            #region Implementation of IDisposable

            /// <summary>
            /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
            /// </summary>
            public void Dispose()
            {
                _eventWaitHandle.Dispose();
            }

            #endregion Implementation of IDisposable
        }

        private sealed class DoneAsyncResult : IAsyncResult
        {
            private readonly object _asyncState;
            private static readonly WaitHandle WaitHandle = new ManualResetEvent(true);

            public DoneAsyncResult(object asyncState)
            {
                _asyncState = asyncState;
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return true; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return true; }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return WaitHandle; }
            }

            object IAsyncResult.AsyncState
            {
                get { return _asyncState; }
            }
        }
    }
}