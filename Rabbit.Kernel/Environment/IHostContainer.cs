namespace Rabbit.Kernel.Environment
{
    /// <summary>
    /// 一个抽象的主机容器。
    /// </summary>
    public interface IHostContainer
    {
        /// <summary>
        /// 解析服务。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>服务实例。</returns>
        T Resolve<T>();
    }
}