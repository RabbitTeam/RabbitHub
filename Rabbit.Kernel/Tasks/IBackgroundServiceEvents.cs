using Rabbit.Kernel.Events;
using System;

namespace Rabbit.Kernel.Tasks
{
    /// <summary>
    /// 一个抽象的后台服务事件。
    /// </summary>
    public interface IBackgroundServiceEvents : IEventHandler
    {
        /// <summary>
        /// 开始扫描前执行。
        /// </summary>
        /// <param name="backgroundTask">后台任务实例。</param>
        void Sweeping(IBackgroundTask backgroundTask);

        /// <summary>
        /// 在扫描过程中发生异常时执行。
        /// </summary>
        /// <param name="backgroundTask">后台任务实例。</param>
        /// <param name="exception">异常信息。</param>
        void OnException(IBackgroundTask backgroundTask, Exception exception);

        /// <summary>
        /// 扫描结束时执行。
        /// </summary>
        /// <param name="backgroundTask">后台任务实例。</param>
        void Sweeped(IBackgroundTask backgroundTask);
    }
}