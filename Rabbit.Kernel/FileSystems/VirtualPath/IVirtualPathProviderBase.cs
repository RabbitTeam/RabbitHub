using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rabbit.Kernel.FileSystems.VirtualPath
{
    /// <summary>
    /// 一个抽象的虚拟路径提供者基础。
    /// </summary>
    public interface IVirtualPathProviderBase
    {
        /// <summary>
        /// 组合路径。
        /// </summary>
        /// <param name="paths">路径数组。</param>
        /// <returns>组合之后的路径。</returns>
        string Combine(params string[] paths);

        /// <summary>
        /// 获取虚拟路径的绝对路径。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>绝对路径。</returns>
        string MapPath(string virtualPath);

        /// <summary>
        /// 判断一个虚拟路径的文件是否存在。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>虚拟路径。</returns>
        bool FileExists(string virtualPath);

        /// <summary>
        /// 打开一个文件。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="action">对流的操作。</param>
        /// <returns>文件流。</returns>
        void OpenFile(string virtualPath, Action<Stream> action);

        /// <summary>
        /// 在指定虚拟路径创建一个文件并创建一个写入流。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="action">对写入流的操作。</param>
        /// <returns>新文件的写入流。</returns>
        void CreateText(string virtualPath, Action<StreamWriter> action);

        /// <summary>
        /// 创建一个文件。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="action">对流的操作。</param>
        /// <returns>文件流。</returns>
        void CreateFile(string virtualPath, Action<Stream> action);

        /// <summary>
        /// 获取文件最后的写入UTC时间。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>UTC时间。</returns>
        DateTime? GetFileLastWriteTimeUtc(string virtualPath);

        /// <summary>
        /// 获取目录最后的写入UTC时间。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>UTC时间。</returns>
        DateTime? GetDirectoryLastWriteTimeUtc(string virtualPath);

        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        void DeleteFile(string virtualPath);

        /// <summary>
        /// 判断目录是否存在。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>如果存在则返回true，否则返回false。</returns>
        bool DirectoryExists(string virtualPath);

        /// <summary>
        /// 创建一个目录。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        void CreateDirectory(string virtualPath);

        /// <summary>
        /// 获取目录名称。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>目录名称。</returns>
        string GetDirectoryName(string virtualPath);

        /// <summary>
        /// 删除目录。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        void DeleteDirectory(string virtualPath);

        /// <summary>
        /// 获取指定路径下的所有文件。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="includeChildren">是否包含子级目录。</param>
        /// <returns>文件路径集合。</returns>
        IEnumerable<string> ListFiles(string virtualPath, bool includeChildren = false);

        /// <summary>
        /// 获取指定路径下的所有目录。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="includeChildren">是否包含子级目录。</param>
        /// <returns>目录路径集合。</returns>
        IEnumerable<string> ListDirectories(string virtualPath, bool includeChildren = false);
    }

    /// <summary>
    /// 虚拟路径提供者扩展方法。
    /// </summary>
    public static class VirtualPathProviderExtensions
    {
        /// <summary>
        /// 打开一个文件。
        /// </summary>
        /// <typeparam name="T">返回值类型。</typeparam>
        /// <param name="virtualPathProvider">虚拟路径提供者。</param>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="func">对流的操作。</param>
        /// <returns>文件流。</returns>
        public static T OpenFileFunc<T>(this IVirtualPathProviderBase virtualPathProvider, string virtualPath, Func<Stream, T> func)
        {
            virtualPathProvider.NotNull("virtualPathProvider");
            virtualPath.NotEmptyOrWhiteSpace("virtualPath");
            func.NotNull("func");

            var result = default(T);
            virtualPathProvider.OpenFile(virtualPath, stream =>
            {
                result = func(stream);
            });
            return result;
        }

        /// <summary>
        /// 创建一个文件。
        /// </summary>
        /// <typeparam name="T">返回值类型。</typeparam>
        /// <param name="virtualPathProvider">虚拟路径提供者。</param>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="func">对流的操作。</param>
        /// <returns>文件流。</returns>
        public static T CreateFileFunc<T>(this IVirtualPathProviderBase virtualPathProvider, string virtualPath, Func<Stream, T> func)
        {
            virtualPathProvider.NotNull("virtualPathProvider");
            virtualPath.NotEmptyOrWhiteSpace("virtualPath");
            func.NotNull("func");

            var result = default(T);
            virtualPathProvider.CreateFile(virtualPath, stream => result = func(stream));
            return result;
        }

        /// <summary>
        /// 在指定虚拟路径创建一个文件并创建一个写入流。
        /// </summary>
        /// <typeparam name="T">返回值类型。</typeparam>
        /// <param name="virtualPathProvider">虚拟路径提供者。</param>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="func">对写入流的操作。</param>
        /// <returns>新文件的写入流。</returns>
        public static T CreateTextFunc<T>(this IVirtualPathProviderBase virtualPathProvider, string virtualPath, Func<StreamWriter, T> func)
        {
            virtualPathProvider.NotNull("virtualPathProvider");
            virtualPath.NotEmptyOrWhiteSpace("virtualPath");
            func.NotNull("func");

            var result = default(T);
            virtualPathProvider.CreateText(virtualPath, stream => result = func(stream));
            return result;
        }

        /// <summary>
        /// 获取文件或目录最后的写入UTC时间。
        /// </summary>
        /// <param name="virtualPathProvider">虚拟路径提供者。</param>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>UTC时间。</returns>
        public static DateTime? GetLastWriteTimeUtc(this IVirtualPathProviderBase virtualPathProvider, string virtualPath)
        {
            virtualPathProvider.NotNull("virtualPathProvider");
            virtualPath.NotEmptyOrWhiteSpace("virtualPath");

            var dateTime = virtualPathProvider.GetFileLastWriteTimeUtc(virtualPath);
            return dateTime.HasValue ? dateTime : virtualPathProvider.GetDirectoryLastWriteTimeUtc(virtualPath);
        }
    }
}