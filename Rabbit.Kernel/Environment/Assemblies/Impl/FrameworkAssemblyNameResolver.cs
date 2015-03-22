using Rabbit.Kernel.Caching;
using System;
using System.Linq;
using System.Reflection;

namespace Rabbit.Kernel.Environment.Assemblies.Impl
{
    internal sealed class FrameworkAssemblyNameResolver : IAssemblyNameResolver
    {
        #region Field

        private readonly ICacheManager _cacheManager;

        #endregion Field

        #region Constructor

        public FrameworkAssemblyNameResolver(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion Constructor

        #region Implementation of IAssemblyNameResolver

        /// <summary>
        /// 排序。
        /// </summary>
        public int Order { get { return 20; } }

        /// <summary>
        /// 解析一个程序集短名称到一个完全名称。
        /// </summary>
        /// <param name="shortName">程序集短名称。</param>
        /// <returns>程序集完全名称。</returns>
        public string Resolve(string shortName)
        {
            var frameworkReferences = _cacheManager.Get(typeof(IAssemblyLoader), ctx =>
                   ctx.Key.Assembly
                   .GetReferencedAssemblies()
                   .GroupBy(n => AssemblyLoaderExtensions.ExtractAssemblyShortName(n.FullName), StringComparer.OrdinalIgnoreCase)
                   .ToDictionary(n => n.Key, g => g.OrderBy(n => n.Version).Last(), StringComparer.OrdinalIgnoreCase));

            AssemblyName assemblyName;
            return frameworkReferences.TryGetValue(shortName, out assemblyName) ? assemblyName.FullName : null;
        }

        #endregion Implementation of IAssemblyNameResolver
    }
}