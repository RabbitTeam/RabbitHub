using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Loaders;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.Dependencies;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Logging;
using System;
using System.Linq;

namespace Rabbit.Web.Mvc.Extensions.Loaders
{
    internal sealed class RawThemeExtensionLoader : ExtensionLoaderBase
    {
        #region Field

        private readonly IVirtualPathProvider _virtualPathProvider;

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        public bool Disabled { get; set; }

        #endregion Property

        #region Constructor

        /// <summary>
        /// 初始化一个新的扩展装载机。
        /// </summary>
        /// <param name="dependenciesFolder">依赖文件夹。</param>
        /// <param name="virtualPathProvider">虚拟路提供程序。</param>
        public RawThemeExtensionLoader(IDependenciesFolder dependenciesFolder, IVirtualPathProvider virtualPathProvider)
            : base(dependenciesFolder)
        {
            _virtualPathProvider = virtualPathProvider;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Overrides of ExtensionLoaderBase

        /// <summary>
        /// 排序。
        /// </summary>
        public override int Order
        {
            get { return 10; }
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

            if (descriptor.Location != "~/Themes")
                return null;
            var projectPath = _virtualPathProvider.Combine(descriptor.Location, descriptor.Id,
                descriptor.Id + ".csproj");

            //如果项目文件不存在则忽略主题
            if (_virtualPathProvider.FileExists(projectPath))
            {
                return null;
            }

            var assemblyPath = _virtualPathProvider.Combine(descriptor.Location, descriptor.Id, "bin",
                descriptor.Id + ".dll");

            //如果主题包含dll文件则忽略
            if (_virtualPathProvider.FileExists(assemblyPath))
                return null;

            return new ExtensionProbeEntry
            {
                Descriptor = descriptor,
                Loader = this,
                VirtualPath = "~/Theme/" + descriptor.Id,
                VirtualPathDependencies = Enumerable.Empty<string>(),
            };
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

            Logger.Information("加载没有代码的主题 \"{0}\"", descriptor.Descriptor.Name);

            return new ExtensionEntry
            {
                Descriptor = descriptor,
                Assembly = GetType().Assembly,
                ExportedTypes = new Type[0]
            };
        }

        #endregion Overrides of ExtensionLoaderBase
    }
}