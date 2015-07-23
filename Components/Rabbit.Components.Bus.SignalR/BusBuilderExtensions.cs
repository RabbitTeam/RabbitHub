using Autofac;
using Rabbit.Kernel.Bus;
using Rabbit.Kernel.Utility.Extensions;

namespace Rabbit.Components.Bus.SignalR
{
    /// <summary>
    /// 总线建设者扩展方法。
    /// </summary>
    public static class BusBuilderExtensions
    {
        /// <summary>
        /// 主机 Url。
        /// </summary>
        internal static string HostUrl;

        internal static string Path;

        /// <summary>
        /// 使用 SignalR 总线。
        /// </summary>
        /// <param name="busBuilder">总线建设者。</param>
        /// <param name="hostUrl">主机 Url。</param>
        /// <param name="path">路径。</param>
        public static void UseSignalR(this BuilderExtensions.IBusBuilder busBuilder, string hostUrl, string path)
        {
            HostUrl = hostUrl.NotEmptyOrWhiteSpace("hostUrl");
            Path = path.NotEmptyOrWhiteSpace("path");

            busBuilder.KernelBuilder.OnStarting(builder => builder.RegisterType<SignalRBus>().As<IBus>().InstancePerMatchingLifetimeScope("shell"));
        }
    }
}