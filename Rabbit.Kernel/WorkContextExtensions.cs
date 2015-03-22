using Autofac;
using Rabbit.Kernel.Works;

namespace Rabbit.Kernel
{
    /// <summary>
    /// 工作上下文扩展方法。
    /// </summary>
    public static class WorkContextExtensions
    {
        /// <summary>
        /// 创建一个工作上下文范围。
        /// </summary>
        /// <param name="lifetimeScope">容器。</param>
        /// <returns>工作上下文范围。</returns>
        public static IWorkContextScope CreateWorkContextScope(this ILifetimeScope lifetimeScope)
        {
            return lifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope();
        }
    }
}