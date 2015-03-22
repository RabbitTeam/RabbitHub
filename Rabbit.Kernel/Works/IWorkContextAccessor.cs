namespace Rabbit.Kernel.Works
{
    /// <summary>
    /// 一个抽象的工作上下文访问器。
    /// </summary>
    public interface IWorkContextAccessor
    {
        /// <summary>
        /// 获取工作上下文。
        /// </summary>
        /// <returns>工作上下文。</returns>
        WorkContext GetContext();

        /// <summary>
        /// 创建一个工作上下文范围。
        /// </summary>
        /// <returns>工作上下文范围。</returns>
        IWorkContextScope CreateWorkContextScope();
    }
}