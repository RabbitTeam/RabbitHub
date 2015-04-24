using Rabbit.Kernel.Environment;
using System;
using System.Threading;
using System.Web.Razor.Generator;
using System.Web.WebPages.Razor;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.Razor
{
    internal sealed class RazorCompilationEventsShim : IShim
    {
        private static int _initialized;

        private RazorCompilationEventsShim()
        {
            HostContainerRegistry.RegisterShim(this);
            RazorBuildProvider.CodeGenerationStarted += RazorBuildProviderCodeGenerationStarted;
            RazorBuildProvider.CodeGenerationCompleted += RazorBuildProviderCodeGenerationCompleted;
        }

        #region Implementation of IShim

        /// <summary>
        /// 主机容器。
        /// </summary>
        public IHostContainer HostContainer { get; set; }

        #endregion Implementation of IShim

        private void RazorBuildProviderCodeGenerationStarted(object sender, EventArgs e)
        {
            var provider = (RazorBuildProvider)sender;
            HostContainer.Resolve<IRazorCompilationEvents>().CodeGenerationStarted(provider);
        }

        private void RazorBuildProviderCodeGenerationCompleted(object sender, CodeGenerationCompleteEventArgs e)
        {
            var provider = (RazorBuildProvider)sender;
            HostContainer.Resolve<IRazorCompilationEvents>().CodeGenerationCompleted(provider, e);
        }

        public static void EnsureInitialized()
        {
            var uninitialized = Interlocked.CompareExchange(ref _initialized, 1, 0) == 0;
            if (uninitialized)
                new RazorCompilationEventsShim();
        }
    }
}