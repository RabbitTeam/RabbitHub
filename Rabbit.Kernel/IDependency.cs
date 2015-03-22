using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Logging;

namespace Rabbit.Kernel
{
    /// <summary>
    /// 表示实现者是一个基础依赖。
    /// </summary>
    public interface IDependency
    {
    }

    /// <summary>
    /// 表示实现者是一个单列依赖。
    /// </summary>
    public interface ISingletonDependency : IDependency
    {
    }

    /// <summary>
    /// 表示实现者是一个瞬态依赖。
    /// </summary>
    public interface ITransientDependency : IDependency
    {
    }

    /// <summary>
    /// 表示实现者是一个工作单元依赖。
    /// </summary>
    public interface IUnitOfWorkDependency : IDependency
    {
    }

    /// <summary>
    /// 一个抽象的组件。
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// 初始化一个抽象的组件。
        /// </summary>
        protected Component()
        {
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        /// <summary>
        /// 日志记录器。
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 本地化委托。
        /// </summary>
        public Localizer T { get; set; }
    }
}