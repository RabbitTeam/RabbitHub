using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Assemblies.Models;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.Application;
using Rabbit.Kernel.FileSystems.Dependencies;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rabbit.Kernel.Extensions.Loaders.Impl
{
    internal sealed class PrecompiledExtensionLoader : ExtensionLoaderBase
    {
        #region Field

        private readonly IHostEnvironment _hostEnvironment;
        private readonly IAssemblyProbingFolder _assemblyProbingFolder;
        private readonly IApplicationFolder _applicationFolder;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的扩展装载机。
        /// </summary>
        /// <param name="dependenciesFolder">依赖文件夹。</param>
        /// <param name="assemblyProbingFolder">程序集探测文件夹。</param>
        /// <param name="applicationFolder">应用程序文件夹。</param>
        /// <param name="hostEnvironment">主机环境。</param>
        public PrecompiledExtensionLoader(IDependenciesFolder dependenciesFolder, IHostEnvironment hostEnvironment, IAssemblyProbingFolder assemblyProbingFolder, IApplicationFolder applicationFolder)
            : base(dependenciesFolder)
        {
            _hostEnvironment = hostEnvironment;
            _assemblyProbingFolder = assemblyProbingFolder;
            _applicationFolder = applicationFolder;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        public bool Disabled { get; set; }

        public bool DisableMonitoring { get; set; }

        #endregion Property

        #region Overrides of ExtensionLoaderBase

        /// <summary>
        /// 排序。
        /// </summary>
        public override int Order
        {
            get { return 30; }
        }

        /// <summary>
        /// 探测引用。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <returns>扩展音域探测条目集合。</returns>
        public override IEnumerable<ExtensionReferenceProbeEntry> ProbeReferences(ExtensionDescriptorEntry descriptor)
        {
            if (Disabled)
                return Enumerable.Empty<ExtensionReferenceProbeEntry>();

            Logger.Information("探测模块 '{0}' 的引用信息", descriptor.Id);

            var assemblyPath = GetAssemblyPath(descriptor);
            if (assemblyPath == null)
                return Enumerable.Empty<ExtensionReferenceProbeEntry>();

            var result = _applicationFolder
                .ListFiles(_applicationFolder.GetDirectoryName(assemblyPath))
                .Where(s => StringComparer.OrdinalIgnoreCase.Equals(Path.GetExtension(s), ".dll"))
                .Where(s => !StringComparer.OrdinalIgnoreCase.Equals(Path.GetFileNameWithoutExtension(s), descriptor.Id))
                .Select(path => new ExtensionReferenceProbeEntry
                {
                    Descriptor = descriptor,
                    Loader = this,
                    Name = Path.GetFileNameWithoutExtension(path),
                    VirtualPath = path
                })
                .ToList();

            Logger.Information("完成模块 '{0}' 的引用探测", descriptor.Id);
            return result;
        }

        /// <summary>
        /// 装载引用。
        /// </summary>
        /// <param name="reference">引用描述符。</param>
        /// <returns>程序集。</returns>
        public override Assembly LoadReference(DependencyReferenceDescriptor reference)
        {
            if (Disabled)
                return null;

            Logger.Information("加载引用 '{0}'", reference.Name);

            var result = _assemblyProbingFolder.LoadAssembly(new AssemblyDescriptor(reference.Name));

            Logger.Information("完成加载引用 '{0}'", reference.Name);
            return result;
        }

        /// <summary>
        /// 激活引用。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="referenceEntry">引用条目。</param>
        public override void ReferenceActivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
        {
            if (string.IsNullOrEmpty(referenceEntry.VirtualPath))
                return;

            var sourceFileName = _applicationFolder.MapPath(referenceEntry.VirtualPath);

            //如果程序集文件不存在或者比新的旧则复制新的程序集文件到依赖目录。
            var copyAssembly =
                !_assemblyProbingFolder.AssemblyExists(new AssemblyDescriptor(referenceEntry.Name)) ||
                File.GetLastWriteTimeUtc(sourceFileName) > _assemblyProbingFolder.GetAssemblyDateTimeUtc(new AssemblyDescriptor(referenceEntry.Name));

            if (!copyAssembly)
                return;
            context.CopyActions.Add(() => _assemblyProbingFolder.StoreAssembly(new AssemblyDescriptor(referenceEntry.Name), sourceFileName));

            //如果程序集已经被加载，需要重新启动AppDomain
            if (!_hostEnvironment.IsAssemblyLoaded(referenceEntry.Name))
                return;
            Logger.Information("ReferenceActivated: 引用 \"{0}\" 激活新的程序集文件加载，迫使AppDomain重启", referenceEntry.Name);
            context.RestartAppDomain = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <param name="references">扩展探测条目集合。</param>
        /// <returns></returns>
        public override bool IsCompatibleWithModuleReferences(ExtensionDescriptorEntry descriptor, IEnumerable<ExtensionProbeEntry> references)
        {
            return true;
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

            Logger.Information("探测模块 '{0}'", descriptor.Id);

            var assemblyPath = GetAssemblyPath(descriptor);
            if (assemblyPath == null)
                return null;

            var result = new ExtensionProbeEntry
            {
                Descriptor = descriptor,
                Loader = this,
                VirtualPath = assemblyPath,
                VirtualPathDependencies = new[] { assemblyPath },
            };

            Logger.Information("完成模块 '{0}' 探测", descriptor.Id);
            return result;
        }

        /// <summary>
        /// 扩展激活。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="descriptor">扩展描述符条目。</param>
        public override void ExtensionActivated(ExtensionLoadingContext context, ExtensionDescriptorEntry descriptor)
        {
            var sourceFileName = _applicationFolder.MapPath(GetAssemblyPath(descriptor));

            //如果程序集文件不存在或者比新的旧则复制新的程序集文件到依赖目录。
            var copyAssembly =
                !_assemblyProbingFolder.AssemblyExists(new AssemblyDescriptor(descriptor.Id)) ||
                File.GetLastWriteTimeUtc(sourceFileName) > _assemblyProbingFolder.GetAssemblyDateTimeUtc(new AssemblyDescriptor(descriptor.Id));

            if (!copyAssembly)
                return;
            context.CopyActions.Add(() => _assemblyProbingFolder.StoreAssembly(new AssemblyDescriptor(descriptor.Id), sourceFileName));

            //如果程序集已经被加载，需要重新启动AppDomain
            if (!_hostEnvironment.IsAssemblyLoaded(descriptor.Id))
                return;
            Logger.Information("ExtensionRemoved: 模块 \"{0}\" 激活新的程序集文件加载，迫使AppDomain重启", descriptor.Id);
            context.RestartAppDomain = true;
        }

        /// <summary>
        /// 扩展停用。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="descriptor">扩展描述符条目。</param>
        public override void ExtensionDeactivated(ExtensionLoadingContext context, ExtensionDescriptorEntry descriptor)
        {
            if (!_assemblyProbingFolder.AssemblyExists(new AssemblyDescriptor(descriptor.Id)))
                return;

            context.DeleteActions.Add(
                () =>
                {
                    Logger.Information("ExtensionDeactivated: 删除在探测目录中的程序集 \"{0}\"", descriptor.Id);
                    _assemblyProbingFolder.DeleteAssembly(descriptor.Id);
                });

            //如果程序集已经被加载，需要重新启动AppDomain
            if (!_hostEnvironment.IsAssemblyLoaded(descriptor.Id))
                return;
            Logger.Information("ExtensionDeactivated: 模块 \"{0}\" 已停用，它的程序集已经被加载，迫使AppDomain重启", descriptor.Id);
            context.RestartAppDomain = true;
        }

        /// <summary>
        /// 扩展删除。
        /// </summary>
        /// <param name="context">扩展装载上下文。</param>
        /// <param name="dependency">依赖项描述符。</param>
        public override void ExtensionRemoved(ExtensionLoadingContext context, DependencyDescriptor dependency)
        {
            if (!_assemblyProbingFolder.AssemblyExists(new AssemblyDescriptor(dependency.Name)))
                return;

            context.DeleteActions.Add(
                () =>
                {
                    Logger.Information("ExtensionRemoved: 删除在探测目录中的程序集 \"{0}\"", dependency.Name);
                    _assemblyProbingFolder.DeleteAssembly(dependency.Name);
                });

            //如果程序集已经被加载，需要重新启动AppDomain
            if (!_hostEnvironment.IsAssemblyLoaded(dependency.Name))
                return;
            Logger.Information("ExtensionRemoved: 模块\"{0}\"被删除，它的程序集加载，迫使AppDomain重启", dependency.Name);
            context.RestartAppDomain = true;
        }

        /// <summary>
        /// 监控扩展。
        /// </summary>
        /// <param name="descriptor">扩展描述符条目。</param>
        /// <param name="monitor">监控动作。</param>
        public override void Monitor(ExtensionDescriptorEntry descriptor, Action<IVolatileToken> monitor)
        {
            if (Disabled)
                return;

            if (DisableMonitoring)
                return;

            //如果程序集文件存在则进行监控。
            var assemblyPath = GetAssemblyPath(descriptor);
            if (assemblyPath != null)
            {
                Logger.Debug("监控虚拟路径 \"{0}\"", assemblyPath);
                monitor(_applicationFolder.WhenPathChanges(assemblyPath));
                return;
            }

            //如果该组件不存在，我们监测含有“bin”文件夹后，如果是在Visual Studio中，例如重新编译的组件可能存在，我们需要改变配置检测。
            var assemblyDirectory = _applicationFolder.Combine(descriptor.Location, descriptor.Id, "bin");
            if (!_applicationFolder.DirectoryExists(assemblyDirectory))
                return;
            Logger.Debug("监控虚拟路径 \"{0}\"", assemblyDirectory);
            monitor(_applicationFolder.WhenPathChanges(assemblyDirectory));
        }

        /// <summary>
        /// 获取编译引用信息。
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns>扩展编译引用集合。</returns>
        public override IEnumerable<ExtensionCompilationReference> GetCompilationReferences(DependencyDescriptor dependency)
        {
            yield return new ExtensionCompilationReference { AssemblyName = dependency.Name };
        }

        /// <summary>
        /// 获取依赖文件的虚拟路径。
        /// </summary>
        /// <param name="dependency">依赖项描述符。</param>
        /// <returns>虚拟路径集合。</returns>
        public override IEnumerable<string> GetVirtualPathDependencies(DependencyDescriptor dependency)
        {
            yield return _assemblyProbingFolder.GetAssemblyVirtualPath(new AssemblyDescriptor(dependency.Name));
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

            Logger.Information("开始加载预编译的扩展 \"{0}\"", descriptor.Descriptor.Name);

            var assembly = _assemblyProbingFolder.LoadAssembly(new AssemblyDescriptor(descriptor.Id));
            if (assembly == null)
                return null;

            Logger.Information("完成加载预编译的扩展 \"{0}\": 程序集名称=\"{1}\"", descriptor.Descriptor.Name, assembly.FullName);

            return new ExtensionEntry
            {
                Descriptor = descriptor,
                Assembly = assembly,
                ExportedTypes = assembly.GetTypes()
            };
        }

        #endregion Overrides of ExtensionLoaderBase

        #region Private Method

        public string GetAssemblyPath(ExtensionDescriptorEntry descriptor)
        {
            var assemblyPath = _applicationFolder.Combine(descriptor.Location, descriptor.Id, "bin",
                                                            descriptor.Id + ".dll");
            return !_applicationFolder.FileExists(assemblyPath) ? null : assemblyPath;
        }

        #endregion Private Method
    }
}