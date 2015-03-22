namespace Rabbit.Kernel.Environment
{
    /// <summary>
    /// 一个抽象的外壳事件。
    /// </summary>
    public interface IShellEvents : IDependency
    {
        /// <summary>
        /// 激活外壳完成后执行。
        /// </summary>
        void Activated();

        /// <summary>
        /// 终止外壳前候执行。
        /// </summary>
        void Terminating();
    }
}