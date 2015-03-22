namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 一个抽象的信号量挥发提供程序。
    /// </summary>
    public interface ISignals : IVolatileProvider
    {
        /// <summary>
        /// 触发信号。
        /// </summary>
        /// <typeparam name="T">信号类型。</typeparam>
        /// <param name="signal">信号值。</param>
        void Trigger<T>(T signal);

        /// <summary>
        /// 根据 <paramref name="signal"/> 获取挥发令牌。
        /// </summary>
        /// <typeparam name="T">信号类型。</typeparam>
        /// <param name="signal">信号值。</param>
        /// <returns>挥发令牌。</returns>
        IVolatileToken When<T>(T signal);
    }
}