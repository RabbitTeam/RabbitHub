using Rabbit.Kernel.Settings;

namespace Rabbit.Kernel.Works
{
    /// <summary>
    /// 一个抽象的工作上下文。
    /// </summary>
    public abstract class WorkContext
    {
        /// <summary>
        /// 解析一个服务。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>服务实例。</returns>
        public abstract T Resolve<T>();

        /// <summary>
        /// 尝试解析一个服务。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <param name="service">服务实例。</param>
        /// <returns>如果解析成功则返回true，否则返回false。</returns>
        public abstract bool TryResolve<T>(out T service);

        /// <summary>
        /// 获取一个状态。
        /// </summary>
        /// <typeparam name="T">状态类型。</typeparam>
        /// <param name="name">状态名称。</param>
        /// <returns>状态实例。</returns>
        public abstract T GetState<T>(string name);

        /// <summary>
        /// 设置一个状态。
        /// </summary>
        /// <typeparam name="T">状态类型。</typeparam>
        /// <param name="name">状态名称。</param>
        /// <param name="value">状态值。</param>
        public abstract void SetState<T>(string name, T value);

        /// <summary>
        /// 工作上下文的文化。
        /// </summary>
        public string CurrentCulture
        {
            get { return GetState<string>("CurrentCulture"); }
            set { SetState("CurrentCulture", value); }
        }

        /// <summary>
        /// 该租户设置相关的内容。
        /// </summary>
        public ITenant CurrentTenant
        {
            get { return GetState<ITenant>("CurrentTenant"); }
            set { SetState("CurrentTenant", value); }
        }
    }
}