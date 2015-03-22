using Rabbit.Kernel.Environment.Assemblies;
using Rabbit.Kernel.Environment.Assemblies.Models;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.AppData;
using Rabbit.Kernel.FileSystems.Application;
using Rabbit.Kernel.Logging;
using System;
using System.Reflection;

namespace Rabbit.Kernel.FileSystems.Dependencies.Impl
{
    internal sealed class DefaultAssemblyProbingFolder : IAssemblyProbingFolder
    {
        #region Field

        private const string BasePath = "Dependencies";
        private readonly IAppDataFolder _appDataFolder;
        private readonly IAssemblyLoader _assemblyLoader;
        private readonly IExtensionManager _extensionManager;
        private readonly IApplicationFolder _applicationFolder;

        #endregion Field

        #region Constructor

        public DefaultAssemblyProbingFolder(IAppDataFolder appDataFolder, IAssemblyLoader assemblyLoader, IExtensionManager extensionManager, IApplicationFolder applicationFolder)
        {
            _appDataFolder = appDataFolder;
            _assemblyLoader = assemblyLoader;
            _extensionManager = extensionManager;
            _applicationFolder = applicationFolder;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IAssemblyProbingFolder

        /// <summary>
        /// 程序集是否存在。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <returns>true为存在，false为不存在。</returns>
        public bool AssemblyExists(AssemblyDescriptor descriptor)
        {
            var path = PrecompiledAssemblyPath(descriptor);
            return _appDataFolder.FileExists(path);
        }

        /// <summary>
        /// 获取程序集的最后修改的Utc时间，如果不存在则返回null。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <returns>Utc时间。</returns>
        public DateTime? GetAssemblyDateTimeUtc(AssemblyDescriptor descriptor)
        {
            var path = PrecompiledAssemblyPath(descriptor);
            return _appDataFolder.GetFileLastWriteTimeUtc(path);
        }

        /// <summary>
        /// 获取程序集的虚拟路径。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <returns>虚拟路径。</returns>
        public string GetAssemblyVirtualPath(AssemblyDescriptor descriptor)
        {
            var path = PrecompiledAssemblyPath(descriptor);
            return !_appDataFolder.FileExists(path) ? null : _appDataFolder.GetVirtualPath(path);
        }

        /// <summary>
        /// 装载程序集。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <returns>程序集。</returns>
        public Assembly LoadAssembly(AssemblyDescriptor descriptor)
        {
            var path = PrecompiledAssemblyPath(descriptor);
            return !_appDataFolder.FileExists(path) ? null : _assemblyLoader.Load(descriptor);
        }

        /// <summary>
        /// 删除程序集。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        public void DeleteAssembly(AssemblyDescriptor descriptor)
        {
            var path = PrecompiledAssemblyPath(descriptor);

            if (!_appDataFolder.FileExists(path))
                return;
            Logger.Information("从程序集探测目录删除程序集 {0}", descriptor.ToString());
            _appDataFolder.DeleteFile(path);
        }

        /// <summary>
        /// 存储程序集。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <param name="fileName">程序集文件名称。</param>
        public void StoreAssembly(AssemblyDescriptor descriptor, string fileName)
        {
            var path = PrecompiledAssemblyPath(descriptor);

            Logger.Information("存储程序集 {0} 到程序集探测目录", descriptor.ToString());
            _appDataFolder.StoreFile(fileName, path);
        }

        /// <summary>
        /// 删除程序集。
        /// </summary>
        /// <param name="moduleName">模块名称。</param>
        public void DeleteAssembly(string moduleName)
        {
            var descriptor = _extensionManager.GetExtension(moduleName);

            if (descriptor == null)
                return;

            /*            var paths = GetModuleAssemblyPaths(descriptor);
                        if (paths == null)
                            return;

                        foreach (var assembly in paths.Select(item => item.Key))
                        {
                            Logger.Information("为模块 \"{0}\" 删除来自探测目录的程序集", moduleName);
                            DeleteAssembly(assembly);
                        }*/

            Logger.Information("为模块 \"{0}\" 删除来自探测目录的程序集", moduleName);
            DeleteAssembly(new AssemblyDescriptor(moduleName));
        }

        /// <summary>
        /// 存储程序集。
        /// </summary>
        /// <param name="moduleName">模块名称。</param>
        public void StoreAssembly(string moduleName)
        {
            var descriptor = _extensionManager.GetExtension(moduleName);

            if (descriptor == null)
                return;

            /*            var paths = GetModuleAssemblyPaths(descriptor);
                        if (paths == null)
                            return;

                        foreach (var item in paths)
                        {
                            var assembly = item.Key;
                            var fileName = item.Value;
                            StoreAssembly(assembly, _applicationFolder.MapPath(fileName));
                        }*/
            var path = GetModuleAssemblyPath(descriptor);
            Logger.Information("为模块 \"{1}\" 存储程序集文件 \"{0}\"", path, moduleName);
            var assemblyDescriptor = new AssemblyDescriptor(moduleName);
            StoreAssembly(assemblyDescriptor, _applicationFolder.MapPath(path));
        }

        #endregion Implementation of IAssemblyProbingFolder

        #region Private Method

        private string PrecompiledAssemblyPath(AssemblyDescriptor descriptor)
        {
            return _appDataFolder.Combine(BasePath, descriptor.Name + ".dll");
        }

        /*        private IEnumerable<KeyValuePair<AssemblyDescriptor, string>> GetModuleAssemblyPaths(ExtensionDescriptorEntry descriptor)
                {
                    if (descriptor.Descriptor == null || descriptor.Descriptor.Runtime == null || descriptor.Descriptor.Runtime.Assemblies == null)
                        return null;
                    return descriptor.Descriptor.Runtime.Assemblies.ToDictionary(i => i,
                        v => _applicationFolder.Combine(descriptor.Location, descriptor.Id, "bin", v.Name + ".dll"));
                }*/

        private string GetModuleAssemblyPath(ExtensionDescriptorEntry descriptor)
        {
            return _applicationFolder.Combine(descriptor.Location, descriptor.Id, "bin", descriptor.Id + ".dll");
        }

        #endregion Private Method
    }
}