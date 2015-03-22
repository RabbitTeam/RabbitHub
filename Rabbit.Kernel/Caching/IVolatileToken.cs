namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 一个抽象的挥发令牌。
    /// </summary>
    public interface IVolatileToken
    {
        /// <summary>
        /// 标识缓存是否有效，true为有效，false为失效。
        /// </summary>
        bool IsCurrent { get; }
    }
}