using System;
using System.Linq;

namespace Rabbit.Kernel.Environment.Impl
{
    internal abstract class HostEnvironment : IHostEnvironment
    {
        /// <summary>
        /// 是否以完全信任模式运行。
        /// </summary>
        public bool IsFullTrust
        {
            get { return AppDomain.CurrentDomain.IsHomogenous && AppDomain.CurrentDomain.IsFullyTrusted; }
        }

        /// <summary>
        /// 根据虚拟路径获取物理路径。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>物理路径。</returns>
        public abstract string MapPath(string virtualPath);

        /// <summary>
        /// 根据程序集名称判断程序集是否已经被加载。
        /// </summary>
        /// <param name="assemblyName">程序集名称。</param>
        /// <returns>如果已经被加载则返回true，否则返回false。</returns>
        public bool IsAssemblyLoaded(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any(assembly =>
                string.Equals(assembly.FullName, assemblyName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(assembly.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 重新启动AppDmain。
        /// </summary>
        public abstract void RestartAppDomain();
    }
}