namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 一个抽象的缓存上下文访问器。
    /// </summary>
    public interface ICacheContextAccessor
    {
        /// <summary>
        /// 当前获取上下文。
        /// </summary>
        IAcquireContext Current { get; set; }
    }
}