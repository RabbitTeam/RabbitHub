namespace Rabbit.Kernel.Tasks
{
    /// <summary>
    /// 一个抽象的后台任务。
    /// </summary>
    public interface IBackgroundTask : IDependency
    {
        /// <summary>
        /// 扫描。
        /// </summary>
        void Sweep();
    }
}