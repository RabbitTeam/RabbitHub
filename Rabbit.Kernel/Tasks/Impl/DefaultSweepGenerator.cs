using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Works;
using System;
using System.Threading;

namespace Rabbit.Kernel.Tasks.Impl
{
    internal sealed class DefaultSweepGenerator : ISweepGenerator, IDisposable
    {
        internal sealed class InternalTimer : IDisposable
        {
            #region Field

            private readonly Action _action;
            private TimeSpan _interval;
            private Timer _timer;

            #endregion Field

            #region Property

            public bool Enabled { get; private set; }

            public TimeSpan Interval
            {
                get { return _interval; }
                set
                {
                    _interval = value;
                    _timer.Change(0, _interval.Milliseconds);
                }
            }

            #endregion Property

            #region Constructor

            public InternalTimer(Action action, TimeSpan interval)
            {
                if (interval.Ticks <= 0)
                    throw new ArgumentException("无效的时间。", "interval");

                _interval = interval;
                _action = action;
            }

            #endregion Constructor

            #region Public Method

            public void Start()
            {
                if (_timer != null)
                    throw new Exception("无法重复启动定时器。");
                Enabled = true;
                LazyInitializer.EnsureInitialized(ref _timer, () =>
                {
                    Enabled = true;
                    return new Timer(state => _action(), null, 0, (long)Interval.TotalMilliseconds);
                });
            }

            public void Stop()
            {
                Dispose();
            }

            #endregion Public Method

            #region Implementation of IDisposable

            /// <summary>
            /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
            /// </summary>
            public void Dispose()
            {
                Enabled = false;
                if (_timer == null)
                    return;
                _timer.Dispose();
                _timer = null;
            }

            #endregion Implementation of IDisposable
        }

        #region Field

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly InternalTimer _timer;

        #endregion Field

        #region Constructor

        public DefaultSweepGenerator(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;

            Logger = NullLogger.Instance;

            _timer = new InternalTimer(Elapsed, TimeSpan.FromMinutes(1));
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        public TimeSpan Interval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        #endregion Property

        #region Implementation of ISweepGenerator

        /// <summary>
        /// 激活。
        /// </summary>
        public void Activate()
        {
            lock (_timer)
                _timer.Start();
        }

        /// <summary>
        /// 终止。
        /// </summary>
        public void Terminate()
        {
            lock (_timer)
                _timer.Stop();
        }

        #endregion Implementation of ISweepGenerator

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            _timer.Dispose();
        }

        #endregion Implementation of IDisposable

        #region Private Method

        private void Elapsed()
        {
            if (!_timer.Enabled)
                return;

            //不允许重复进入。
            if (!Monitor.TryEnter(_timer))
                return;

            if (!_timer.Enabled)
                return;

            try
            {
                DoWork();
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "处理后台任务时发现问题。");
            }
            finally
            {
                Monitor.Exit(_timer);
            }
        }

        private void DoWork()
        {
            using (var scope = _workContextAccessor.CreateWorkContextScope())
            {
                var manager = scope.Resolve<IBackgroundService>();
                manager.Sweep();
            }
        }

        #endregion Private Method
    }
}