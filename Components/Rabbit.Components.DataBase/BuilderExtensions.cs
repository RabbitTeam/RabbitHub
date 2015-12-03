using Autofac;
using Rabbit.Components.DataBase.Providers;
using Rabbit.Kernel;

namespace Rabbit.Components.DataBase
{
    /// <summary>
    /// 建设者扩展方法。
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// 使用数据。
        /// </summary>
        /// <param name="kernelBuilder">内核建设者。</param>
        public static void UseDataBase(this IKernelBuilder kernelBuilder)
        {
            //            kernelBuilder.RegisterExtension(typeof (BuilderExtensions).Assembly);
            kernelBuilder.OnStarting(
                builder =>
                {
                    builder.RegisterType<DbConnectionFactory>().As<IDbConnectionFactory>().InstancePerDependency();
                    builder.RegisterType<SqlDbConnectionProvider>().As<IDbConnectionProvider>().SingleInstance();
                });
        }
    }
}