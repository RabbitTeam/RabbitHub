using System;
using System.Linq;

namespace Rabbit.Kernel.Environment.Assemblies.Impl
{
    internal sealed class AppDomainAssemblyNameResolver : IAssemblyNameResolver
    {
        #region Implementation of IAssemblyNameResolver

        /// <summary>
        /// 排序。
        /// </summary>
        public int Order { get { return 10; } }

        /// <summary>
        /// 解析一个程序集短名称到一个完全名称。
        /// </summary>
        /// <param name="shortName">程序集短名称。</param>
        /// <returns>程序集完全名称。</returns>
        public string Resolve(string shortName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => StringComparer.OrdinalIgnoreCase.Equals(shortName, AssemblyLoaderExtensions.ExtractAssemblyShortName(a.FullName)))
                .Select(a => a.FullName)
                .SingleOrDefault();
        }

        #endregion Implementation of IAssemblyNameResolver
    }
}