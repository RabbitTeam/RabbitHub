using Rabbit.Kernel.Environment.Assemblies;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Loaders;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.Dependencies;
using Rabbit.Kernel.Logging;
using System;
using System.Linq;

namespace Rabbit.Web.Mvc.Extensions.Loaders
{
    internal sealed class CoreExtensionLoader : ExtensionLoaderBase
    {
        #region Field

        private const string CoreAssemblyName = "Rabbit.Core";
        private readonly IAssemblyLoader _assemblyLoader;

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
        /// <param name="assemblyLoader">程序集加载器。</param>
        public CoreExtensionLoader(IDependenciesFolder dependenciesFolder, IAssemblyLoader assemblyLoader)
            : base(dependenciesFolder)
        {
            _assemblyLoader = assemblyLoader;

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

            if (descriptor.Location == "~/Core")
            {
                return new ExtensionProbeEntry
                {
                    Descriptor = descriptor,
                    Loader = this,
                    Priority = 100, //更高的优先级因为总是优先于 ~/bin 中的程序集
                    VirtualPath = "~/Core/" + descriptor.Id,
                    VirtualPathDependencies = Enumerable.Empty<string>(),
                };
            }
            return null;
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

            var assembly = _assemblyLoader.Load(CoreAssemblyName);
            if (assembly == null)
            {
                Logger.Error("不能激活的核心模块，因为程序集 '{0}' 不能加载", CoreAssemblyName);
                return null;
            }

            Logger.Information("加载的核心模块 '{0}'： 程序集名称='{1}'", descriptor.Descriptor.Name, assembly.FullName);

            return new ExtensionEntry
            {
                Descriptor = descriptor,
                Assembly = assembly,
                ExportedTypes = assembly.GetTypes().Where(x => IsTypeFromModule(x, descriptor))
            };
        }

        #endregion Overrides of ExtensionLoaderBase

        #region Private Method

        private static bool IsTypeFromModule(Type type, ExtensionDescriptorEntry descriptor)
        {
            return (type.Namespace + ".").StartsWith(CoreAssemblyName + "." + descriptor.Id + ".");
        }

        #endregion Private Method
    }
}