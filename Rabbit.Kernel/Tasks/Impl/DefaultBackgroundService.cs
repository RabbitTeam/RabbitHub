using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.Kernel.Tasks.Impl
{
    internal sealed class DefaultBackgroundService : IBackgroundService
    {
        #region Field

        private readonly IEnumerable<IBackgroundTask> _tasks;
        private readonly IEnumerable<IBackgroundServiceEvents> _backgroundServiceEventses;

        #endregion Field

        #region Constructor

        public DefaultBackgroundService(IEnumerable<IBackgroundTask> tasks, IEnumerable<IBackgroundServiceEvents> backgroundServiceEventses)
        {
            _tasks = tasks;
            _backgroundServiceEventses = backgroundServiceEventses;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IBackgroundService

        /// <summary>
        /// 扫描。
        /// </summary>
        public void Sweep()
        {
            Action<Action<IBackgroundServiceEvents>> execution = ac =>
            {
                if (!_backgroundServiceEventses.Any())
                    return;
                Parallel.ForEach(_backgroundServiceEventses, ac);
            };

            foreach (var task in _tasks)
            {
                var taskProxy = task;
                try
                {
                    //扫描前执行。
                    execution(i => i.Sweeping(taskProxy));
                    task.Sweep();
                }
                catch (Exception exception)
                {
                    //发送异常时执行。
                    execution(i => i.OnException(taskProxy, exception));
                    Logger.Error(exception, "处理后台任务时候发送了异常。");
                }
                //扫描结束时执行。
                execution(i => i.Sweeped(taskProxy));
            }
        }

        #endregion Implementation of IBackgroundService
    }
}