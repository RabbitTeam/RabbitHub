using Rabbit.Kernel.Environment;

namespace Rabbit.Web.Environment
{
    /// <summary>
    /// 一个抽象的Web主机。
    /// </summary>
    public interface IWebHost : IHost
    {
        /// <summary>
        /// 请求开始时执行。
        /// </summary>
        void BeginRequest();

        /// <summary>
        /// 请求结束时候执行。
        /// </summary>
        void EndRequest();
    }
}