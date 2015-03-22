namespace Rabbit.Kernel.Environment
{
    /// <summary>
    /// 一个抽象的主机运行环境。
    /// </summary>
    public interface IHostEnvironment
    {
        /// <summary>
        /// 是否以完全信任模式运行。
        /// </summary>
        bool IsFullTrust { get; }

        /// <summary>
        /// 根据虚拟路径获取物理路径。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>物理路径。</returns>
        string MapPath(string virtualPath);

        /// <summary>
        /// 根据程序集名称判断程序集是否已经被加载。
        /// </summary>
        /// <param name="assemblyName">程序集名称。</param>
        /// <returns>如果已经被加载则返回true，否则返回false。</returns>
        bool IsAssemblyLoaded(string assemblyName);

        /// <summary>
        /// 重新启动AppDmain。
        /// </summary>
        void RestartAppDomain();
    }
}