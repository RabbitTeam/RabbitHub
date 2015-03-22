namespace Rabbit.Kernel.Environment
{
    /// <summary>
    /// 一个抽象的垫片。
    /// </summary>
    public interface IShim
    {
        /// <summary>
        /// 主机容器。
        /// </summary>
        IHostContainer HostContainer { get; set; }
    }
}