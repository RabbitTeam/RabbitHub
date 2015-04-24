using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Assemblies;
using Rabbit.Kernel.Extensions.Loaders;
using Rabbit.Kernel.FileSystems.Dependencies;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Razor.Generator;
using System.Web.WebPages.Razor;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.Razor.Impl
{
    internal sealed class DefaultRazorCompilationEvents : IRazorCompilationEvents
    {
        #region Field

        private readonly IAssemblyLoader _assemblyLoader;
        private readonly IBuildManager _buildManager;
        private readonly IDependenciesFolder _dependenciesFolder;
        private readonly IExtensionDependenciesManager _extensionDependenciesManager;
        private readonly IEnumerable<IExtensionLoader> _loaders;

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        public DefaultRazorCompilationEvents(
            IDependenciesFolder dependenciesFolder,
            IExtensionDependenciesManager extensionDependenciesManager,
            IBuildManager buildManager,
            IEnumerable<IExtensionLoader> loaders,
            IAssemblyLoader assemblyLoader)
        {
            _dependenciesFolder = dependenciesFolder;
            _extensionDependenciesManager = extensionDependenciesManager;
            _buildManager = buildManager;
            _loaders = loaders;
            _assemblyLoader = assemblyLoader;
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IRazorCompilationEvents

        public void CodeGenerationStarted(RazorBuildProvider provider)
        {
            var assembliesToAdd = new List<Assembly>();

            var moduleDependencyDescriptor = GetModuleDependencyDescriptor(provider.VirtualPath);

            var dependencyDescriptors = _dependenciesFolder.LoadDescriptors();
            List<DependencyDescriptor> filteredDependencyDescriptors;
            if (moduleDependencyDescriptor != null)
            {
                //添加模块
                filteredDependencyDescriptors = new List<DependencyDescriptor> { moduleDependencyDescriptor };

                //添加模块引用
                filteredDependencyDescriptors.AddRange(moduleDependencyDescriptor.References
                    .Select(reference => dependencyDescriptors
                        .FirstOrDefault(dependency => dependency.Name == reference.Name)
                                         ?? new DependencyDescriptor
                                         {
                                             LoaderName = reference.LoaderName,
                                             Name = reference.Name,
                                             VirtualPath = reference.VirtualPath
                                         }
                    ));
            }
            else
            {
                filteredDependencyDescriptors = dependencyDescriptors.ToList();
            }

            var entries = filteredDependencyDescriptors
                .SelectMany(descriptor => _loaders
                    .Where(loader => descriptor.LoaderName == loader.Name)
                    .Select(loader => new
                    {
                        loader,
                        descriptor,
                        references = loader.GetCompilationReferences(descriptor),
                        dependencies = _extensionDependenciesManager.GetVirtualPathDependencies(descriptor.Name)
                    })).ToArray();

            //添加程序集
            foreach (var entry in entries)
            {
                foreach (var reference in entry.references)
                {
                    if (!string.IsNullOrEmpty(reference.AssemblyName))
                    {
                        var assembly = _assemblyLoader.Load(reference.AssemblyName);
                        if (assembly != null)
                            assembliesToAdd.Add(assembly);
                    }
                    if (!string.IsNullOrEmpty(reference.BuildProviderTarget))
                    {
                        //返回总成可能是null，如果。csproj文件不containt任何cs文件，例如
                        var assembly = _buildManager.GetCompiledAssembly(reference.BuildProviderTarget);
                        if (assembly != null)
                            assembliesToAdd.Add(assembly);
                    }
                }
            }

            foreach (var assembly in assembliesToAdd)
            {
                provider.AssemblyBuilder.AddAssemblyReference(assembly);
            }

            //添加虚拟路径依赖（源文件）
            var virtualDependencies = entries
                .SelectMany(e => e.dependencies)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var virtualDependency in virtualDependencies)
            {
                provider.AddVirtualPathDependency(virtualDependency);
            }

            //日志。
            if (!Logger.IsEnabled(LogLevel.Debug))
                return;
            Logger.Debug("CodeGenerationStarted(\"{0}\") - Dependencies: ", provider.VirtualPath);
            foreach (var virtualPath in provider.VirtualPathDependencies)
                Logger.Debug("  VirtualPath: \"{0}\"", virtualPath);
            foreach (var assembly in assembliesToAdd)
                Logger.Debug("  Reference: \"{0}\"", assembly);
        }

        public void CodeGenerationCompleted(RazorBuildProvider provider, CodeGenerationCompleteEventArgs e)
        {
        }

        #endregion Implementation of IRazorCompilationEvents

        #region Private Method

        private DependencyDescriptor GetModuleDependencyDescriptor(string virtualPath)
        {
            var appRelativePath = VirtualPathUtility.ToAppRelative(virtualPath);
            var prefix = PrefixMatch(appRelativePath, new[] { "~/Modules/", "~/Core/" });
            if (prefix == null)
                return null;

            var moduleName = ModuleMatch(appRelativePath, prefix);
            return moduleName == null ? null : _dependenciesFolder.GetDescriptor(moduleName);
        }

        private static string ModuleMatch(string virtualPath, string prefix)
        {
            var index = virtualPath.IndexOf('/', prefix.Length, virtualPath.Length - prefix.Length);
            if (index < 0)
                return null;

            var moduleName = virtualPath.Substring(prefix.Length, index - prefix.Length);
            return (string.IsNullOrEmpty(moduleName) ? null : moduleName);
        }

        private static string PrefixMatch(string virtualPath, params string[] prefixes)
        {
            return prefixes
                .FirstOrDefault(p => virtualPath.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Private Method
    }
}