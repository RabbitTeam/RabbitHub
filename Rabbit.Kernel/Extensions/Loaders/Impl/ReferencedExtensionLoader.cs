using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.Dependencies;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Logging;

namespace Rabbit.Kernel.Extensions.Loaders.Impl
{
    internal sealed class ReferencedExtensionLoader : ExtensionLoaderBase
    {
        #region Field

        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IBuildManager _buildManager;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的扩展装载机。
        /// </summary>
        /// <param name="dependenciesFolder">依赖文件夹。</param>
        /// <param name="virtualPathProvider">虚拟路径提供程序。</param>
        /// <param name="buildManager">生成管理者。</param>
        public ReferencedExtensionLoader(IDependenciesFolder dependenciesFolder, IVirtualPathProvider virtualPathProvider, IBuildManager buildManager)
            : base(dependenciesFolder)
        {
            _virtualPathProvider = virtualPathProvider;
            _buildManager = buildManager;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        public bool Disabled { get; set; }

        #endregion Property

        #region Overrides of ExtensionLoaderBase

        /// <summary>
        /// 排序。
        /// </summary>
        public override int Order
        {
            get { return 20; }
        }

        /// <summary>
        /// 探测。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展探测条目。</returns>
        public override ExtensionProbeEntry Probe(ExtensionDescriptorEntry descriptor)
        {
            if (Disabled)
                return null;

            var assembly = _buildManager.GetReferencedAssembly(descriptor.Id);
            if (assembly == null)
                return null;

            var assemblyPath = _virtualPathProvider.Combine("~/bin", descriptor.Id + ".dll");

            return new ExtensionProbeEntry
            {
                Descriptor = descriptor,
                Loader = this,
                Priority = 100, //更高的优先级，因为在~/bin中的程序集总是优先
                VirtualPath = assemblyPath,
                VirtualPathDependencies = new[] { assemblyPath },
            };
        }

        /// <summary>
        /// 扩展停用。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="descriptor">扩展描述符条目。</param>
        public override void ExtensionDeactivated(ExtensionLoadingContext context, ExtensionDescriptorEntry descriptor)
        {
            DeleteAssembly(context, descriptor.Id);
        }

        /// <summary>
        /// 扩展删除。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="dependency">依赖项描述符。</param>
        public override void ExtensionRemoved(ExtensionLoadingContext context, DependencyDescriptor dependency)
        {
            DeleteAssembly(context, dependency.Name);
        }

        /// <summary>
        /// 装载工作。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展条目。</returns>
        protected override ExtensionEntry LoadWorker(ExtensionDescriptorEntry descriptor)
        {
            if (Disabled)
                return null;

            var assembly = _buildManager.GetReferencedAssembly(descriptor.Id);
            if (assembly == null)
                return null;

            Logger.Information("装载引用扩展 \"{0}\"：程序集名称=\"{1}\"", descriptor.Descriptor.Name, assembly.FullName);

            return new ExtensionEntry
            {
                Descriptor = descriptor,
                Assembly = assembly,
                ExportedTypes = assembly.GetTypes()
            };
        }

        #endregion Overrides of ExtensionLoaderBase

        #region Private Method

        private void DeleteAssembly(ExtensionLoadingContext ctx, string moduleName)
        {
            var assemblyPath = _virtualPathProvider.Combine("~/bin", moduleName + ".dll");
            if (!_virtualPathProvider.FileExists(assemblyPath))
                return;
            ctx.DeleteActions.Add(
                () =>
                {
                    Logger.Information("ExtensionRemoved: 从 bin 目录删除程序集 \"{0}\"（AppDomain将重新启动）", moduleName);
                    _virtualPathProvider.DeleteFile(assemblyPath);
                });
            ctx.RestartAppDomain = true;
        }

        #endregion Private Method
    }
}