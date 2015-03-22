namespace Rabbit.Kernel.Works
{
    /// <summary>
    /// 一个抽象的工作上下文事件。
    /// </summary>
    public interface IWorkContextEvents : IUnitOfWorkDependency
    {
        /// <summary>
        /// 工作上下文启动之后执行。
        /// </summary>
        void Started();

        /// <summary>
        /// 工作上下文完成之后执行。
        /// </summary>
        void Finished();
    }
}