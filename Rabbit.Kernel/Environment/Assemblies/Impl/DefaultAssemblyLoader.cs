using Rabbit.Kernel.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rabbit.Kernel.Environment.Assemblies.Impl
{
    internal sealed class DefaultAssemblyLoader : IAssemblyLoader
    {
        #region Field

        private readonly IEnumerable<IAssemblyNameResolver> _assemblyNameResolvers;
        private readonly ConcurrentDictionary<string, Assembly> _loadedAssemblies = new ConcurrentDictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        public DefaultAssemblyLoader(IEnumerable<IAssemblyNameResolver> assemblyNameResolvers)
        {
            _assemblyNameResolvers = assemblyNameResolvers;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IAssemblyLoader

        /// <summary>
        /// 根据程序集名称装载程序集。
        /// </summary>
        /// <param name="name">程序集名称。</param>
        /// <returns>程序集。</returns>
        public Assembly Load(string name)
        {
            try
            {
                return _loadedAssemblies.GetOrAdd(AssemblyLoaderExtensions.ExtractAssemblyShortName(name), shortName => LoadWorker(shortName, name));
            }
            catch (Exception e)
            {
                Logger.Error(e, "加载程序集 '{0}' 时发生了错误。", name);
#if DEBUG
                throw;
#else
                return null;

#endif
            }
        }

        #endregion Implementation of IAssemblyLoader

        #region Private Method

        private Assembly LoadWorker(string shortName, string fullName)
        {
            Assembly result;

            //尝试加载完全名称。
            if (fullName != shortName)
            {
                result = TryAssemblyLoad(fullName);
                if (result != null)
                    return result;
            }

            //尝试加载短名称。
            result = TryAssemblyLoad(shortName);
            if (result != null)
                return result;

            //尝试将短名称解析成完全名称。
            var resolvedName = _assemblyNameResolvers.Select(r => r.Resolve(shortName)).FirstOrDefault(f => f != null);
            //再一次尝试加载，这一次将抛出异常。
            return Assembly.Load(resolvedName ?? fullName);
        }

        private static Assembly TryAssemblyLoad(string name)
        {
            try
            {
                return Assembly.Load(name);
            }
            catch
            {
                return null;
            }
        }

        #endregion Private Method
    }
}