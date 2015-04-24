using Rabbit.Kernel;

namespace Rabbit.Components.Web.SignalR
{
    /// <summary>
    /// 建设者扩展。
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// 开启 SignalR 功能。
        /// </summary>
        /// <param name="webBuilder">Web建设者。</param>
        public static Rabbit.Web.BuilderExtensions.IWebBuilder EnableSignalR(this Rabbit.Web.BuilderExtensions.IWebBuilder webBuilder)
        {
            webBuilder.KernelBuilder.RegisterExtension(typeof(BuilderExtensions).Assembly);
            return webBuilder;
        }
    }
}