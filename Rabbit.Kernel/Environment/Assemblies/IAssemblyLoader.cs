using Rabbit.Kernel.Environment.Assemblies.Models;
using System;
using System.Reflection;

namespace Rabbit.Kernel.Environment.Assemblies
{
    /// <summary>
    /// 一个抽象的程序集装载机。
    /// </summary>
    public interface IAssemblyLoader
    {
        /// <summary>
        /// 根据程序集名称装载程序集。
        /// </summary>
        /// <param name="name">程序集名称。</param>
        /// <returns>程序集。</returns>
        Assembly Load(string name);
    }

    /// <summary>
    /// 程序集装载机扩展方法。
    /// </summary>
    public static class AssemblyLoaderExtensions
    {
        /// <summary>
        /// 根据程序集描述符装载一个程序集。
        /// </summary>
        /// <param name="assemblyLoader">程序集装载机。</param>
        /// <param name="descriptor">程序集描述符。</param>
        /// <returns>程序集。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="descriptor"/> 为null。</exception>
        public static Assembly Load(this IAssemblyLoader assemblyLoader, AssemblyDescriptor descriptor)
        {
            if (assemblyLoader == null)
                throw new ArgumentNullException("assemblyLoader");
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            return assemblyLoader.Load(descriptor.FullName);
        }

        /// <summary>
        /// 提取程序集短名称。
        /// </summary>
        /// <param name="fullName">程序集完全名称。</param>
        /// <returns>程序集短名称。</returns>
        public static string ExtractAssemblyShortName(string fullName)
        {
            var index = fullName.IndexOf(',');
            return index < 0 ? fullName : fullName.Substring(0, index);
        }
    }
}