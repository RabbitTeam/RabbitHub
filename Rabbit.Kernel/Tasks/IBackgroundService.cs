namespace Rabbit.Kernel.Tasks
{
    /// <summary>
    /// 一个抽象的后台服务。
    /// </summary>
    public interface IBackgroundService : IDependency
    {
        /// <summary>
        /// 扫描。
        /// </summary>
        void Sweep();
    }
}