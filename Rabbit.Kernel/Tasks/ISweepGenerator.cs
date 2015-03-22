namespace Rabbit.Kernel.Tasks
{
    /// <summary>
    /// 一个抽象的扫描发生器。
    /// </summary>
    public interface ISweepGenerator : ISingletonDependency
    {
        /// <summary>
        /// 激活。
        /// </summary>
        void Activate();

        /// <summary>
        /// 终止。
        /// </summary>
        void Terminate();
    }
}