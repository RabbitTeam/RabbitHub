namespace Rabbit.Kernel.Environment
{
    /// <summary>
    /// 一个抽象的外壳接口。
    /// </summary>
    public interface IShell
    {
        /// <summary>
        /// 激活外壳。
        /// </summary>
        void Activate();

        /// <summary>
        /// 终止外壳。
        /// </summary>
        void Terminate();
    }
}