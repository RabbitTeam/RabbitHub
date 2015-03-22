using System;
using System.Collections.Generic;

namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 一个抽象的并行缓存上下文。
    /// </summary>
    public interface IParallelCacheContext
    {
        /// <summary>
        /// 创建一个执行上下文。
        /// </summary>
        /// <typeparam name="T">结果类型。</typeparam>
        /// <param name="function">获取结果的动作。</param>
        /// <returns>任务实例。</returns>
        ITask<T> CreateContextAwareTask<T>(Func<T> function);

        /// <summary>
        /// 以并行的方式运行。
        /// </summary>
        /// <typeparam name="T">源元素类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="source">源集合。</param>
        /// <param name="selector">选择器。</param>
        /// <returns>结果集合。</returns>
        IEnumerable<TResult> RunInParallel<T, TResult>(IEnumerable<T> source, Func<T, TResult> selector);
    }

    /// <summary>
    /// 一个抽象的任务接口。
    /// </summary>
    /// <typeparam name="T">任务的返回类型。</typeparam>
    public interface ITask<out T> : IDisposable
    {
        /// <summary>
        /// 执行任务。
        /// </summary>
        /// <returns>任务结果。</returns>
        T Execute();

        /// <summary>
        /// 执行任务过程中收集到的令牌。
        /// </summary>
        IEnumerable<IVolatileToken> Tokens { get; }

        /// <summary>
        /// 完成任务。
        /// </summary>
        void Finish();
    }
}