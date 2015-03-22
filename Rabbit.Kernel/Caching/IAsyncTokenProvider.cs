using System;

namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 一个抽象的异步象征提供程序。
    /// </summary>
    public interface IAsyncTokenProvider
    {
        /// <summary>
        /// 获取象征。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <returns>挥发象征。</returns>
        IVolatileToken GetToken(Action<Action<IVolatileToken>> task);
    }
}