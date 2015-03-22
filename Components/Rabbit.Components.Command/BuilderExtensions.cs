using Autofac;
using Rabbit.Kernel;
using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Environment.ShellBuilders;
using System.Collections.Generic;

namespace Rabbit.Components.Command
{
    /// <summary>
    ///     建设者扩展方法。
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        ///     使用命令行。
        /// </summary>
        /// <param name="kernelBuilder">内核建设者。</param>
        public static void UseCommand(this IKernelBuilder kernelBuilder)
        {
            kernelBuilder
                .RegisterExtension(typeof(BuilderExtensions).Assembly)
                .OnStarting(builder =>
                    builder.RegisterType<MinimumShellDescriptorProvider>()
                        .As<IMinimumShellDescriptorProvider>()
                        .SingleInstance());
        }

        #region Help Class

        internal sealed class MinimumShellDescriptorProvider : IMinimumShellDescriptorProvider
        {
            #region Implementation of IMinimumShellDescriptorProvider

            /// <summary>
            ///     获取外壳描述符。
            /// </summary>
            /// <param name="features">外壳特性描述符。</param>
            public void GetFeatures(ICollection<ShellFeature> features)
            {
                features.Add(new ShellFeature { Name = "Rabbit.Components.Command" });
            }

            #endregion Implementation of IMinimumShellDescriptorProvider
        }

        #endregion Help Class
    }
}