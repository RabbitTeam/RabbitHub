using System;

namespace Rabbit.Kernel.Works
{
    /// <summary>
    /// 一个抽象的工作上下文范围。
    /// </summary>
    public interface IWorkContextScope : IDisposable
    {
        /// <summary>
        /// 工作上下文。
        /// </summary>
        WorkContext WorkContext { get; }

        /// <summary>
        /// 解析一个服务。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <returns>服务实例。</returns>
        TService Resolve<TService>();

        /// <summary>
        /// 尝试解析一个服务。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="service">服务实例。</param>
        /// <returns>如果解析成功则返回true，否则返回false。</returns>
        bool TryResolve<TService>(out TService service);
    }
}