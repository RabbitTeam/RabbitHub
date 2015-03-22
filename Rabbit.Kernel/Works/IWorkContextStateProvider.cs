using System;

namespace Rabbit.Kernel.Works
{
    /// <summary>
    /// 一个抽象的工作上下文状态提供者。
    /// </summary>
    public interface IWorkContextStateProvider : IDependency
    {
        /// <summary>
        /// 获取状态信值。
        /// </summary>
        /// <typeparam name="T">状态类型。</typeparam>
        /// <param name="name">状态名称。</param>
        /// <returns>获取状态值的委托。</returns>
        Func<WorkContext, T> Get<T>(string name);
    }
}