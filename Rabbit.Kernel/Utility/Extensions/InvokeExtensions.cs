using Rabbit.Kernel.Exceptions;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;

namespace Rabbit.Kernel.Utility.Extensions
{
    /// <summary>
    /// 调用扩展方法。
    /// </summary>
    public static class InvokeExtensions
    {
        #region Public Method

        /// <summary>
        /// 调度一个集合内的所有实例的某个方法。
        /// </summary>
        /// <typeparam name="TEvents">事件类型。</typeparam>
        /// <param name="events">事件集合。</param>
        /// <param name="dispatch">调度动作。</param>
        /// <param name="logger">日志记录器。</param>
        public static void Invoke<TEvents>(this IEnumerable<TEvents> events, Action<TEvents> dispatch, ILogger logger)
        {
            foreach (var sink in events)
            {
                try
                {
                    dispatch(sink);
                }
                catch (Exception ex)
                {
                    if (IsLogged(ex))
                    {
                        logger.Error(ex, "由 {1} 抛出来自 {0} 的异常：{1}",
                            typeof(TEvents).Name,
                            sink.GetType().FullName,
                            ex.GetType().Name);
                    }

                    if (ex.IsFatal())
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 调度一个集合内的所有实例的某个方法并返回一个值。
        /// </summary>
        /// <typeparam name="TEvents">事件类型。</typeparam>
        /// <typeparam name="TResult">返回结果类型。</typeparam>
        /// <param name="events">事件集合。</param>
        /// <param name="dispatch">调度动作。</param>
        /// <param name="logger">日志记录器。</param>
        /// <returns>结果集合。</returns>
        public static IEnumerable<TResult> Invoke<TEvents, TResult>(this IEnumerable<TEvents> events, Func<TEvents, TResult> dispatch, ILogger logger)
        {
            foreach (var sink in events)
            {
                var result = default(TResult);
                try
                {
                    result = dispatch(sink);
                }
                catch (Exception ex)
                {
                    if (IsLogged(ex))
                    {
                        logger.Error(ex, "由 {1} 抛出来自 {0} 的异常：{1}",
                            typeof(TEvents).Name,
                            sink.GetType().FullName,
                            ex.GetType().Name);
                    }

                    if (ex.IsFatal())
                    {
                        throw;
                    }
                }

                yield return result;
            }
        }

        #endregion Public Method

        #region Private Method

        private static bool IsLogged(Exception ex)
        {
            return !ex.IsFatal();
        }

        #endregion Private Method
    }
}