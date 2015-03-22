using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Environment.Assemblies.Models;
using System;
using System.Reflection;

namespace Rabbit.Kernel.FileSystems.Dependencies
{
    /// <summary>
    /// 一个抽象的程序集探测文件夹。
    /// </summary>
    public interface IAssemblyProbingFolder : IVolatileProvider
    {
        /// <summary>
        /// 程序集是否存在。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <returns>true为存在，false为不存在。</returns>
        bool AssemblyExists(AssemblyDescriptor descriptor);

        /// <summary>
        /// 获取程序集的最后修改的Utc时间，如果不存在则返回null。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <returns>Utc时间。</returns>
        DateTime? GetAssemblyDateTimeUtc(AssemblyDescriptor descriptor);

        /// <summary>
        /// 获取程序集的虚拟路径。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <returns>虚拟路径。</returns>
        string GetAssemblyVirtualPath(AssemblyDescriptor descriptor);

        /// <summary>
        /// 装载程序集。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <returns>程序集。</returns>
        Assembly LoadAssembly(AssemblyDescriptor descriptor);

        /// <summary>
        /// 删除程序集。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        void DeleteAssembly(AssemblyDescriptor descriptor);

        /// <summary>
        /// 存储程序集。
        /// </summary>
        /// <param name="descriptor">程序集描述符。</param>
        /// <param name="fileName">程序集文件名称。</param>
        void StoreAssembly(AssemblyDescriptor descriptor, string fileName);

        /// <summary>
        /// 删除程序集。
        /// </summary>
        /// <param name="moduleName">模块名称。</param>
        void DeleteAssembly(string moduleName);

        /// <summary>
        /// 存储程序集。
        /// </summary>
        /// <param name="moduleName">模块名称。</param>
        void StoreAssembly(string moduleName);
    }
}