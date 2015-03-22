using Rabbit.Kernel.Environment;
using Rabbit.Kernel.FileSystems.VirtualPath;
using System;
using System.IO;

namespace Rabbit.Kernel.FileSystems.AppData.Impl
{
    internal sealed class DefaultAppDataFolder : FolderBase, IAppDataFolder
    {
        #region Constructor

        public DefaultAppDataFolder(IHostEnvironment hostEnvironment, IVirtualPathMonitor virtualPathMonitor)
            : base(hostEnvironment, virtualPathMonitor)
        {
        }

        #endregion Constructor

        #region Overrides of VirtualPathProviderBase

        /// <summary>
        /// 根文件夹虚拟路径 ~/ or ~/Abc
        /// </summary>
        public override string RootPath
        {
            get { return "~/App_Data"; }
        }

        /// <summary>
        /// 获取虚拟路径的绝对路径。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>绝对路径。</returns>
        public override string MapPath(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            var tempPath = virtualPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            //如果传入的虚拟路径是一个完整有效的绝对路径则直接返回。
            if (Path.IsPathRooted(tempPath) && !tempPath.StartsWith("\\"))
            {
                return tempPath;
                /*if (tempPath.StartsWith(RootFolder, StringComparison.OrdinalIgnoreCase))
                    return tempPath;
                throw new IOException("无效的虚拟路径。");*/
            }

            virtualPath = virtualPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            //      ~/Abc/test.txt：~/App_Data/Abc/test.txt
            if (virtualPath.StartsWith("~/"))
                virtualPath = virtualPath.Remove(0, 2);

            //是否需要追加分隔符
            var isAppendSeparator = !string.IsNullOrEmpty(virtualPath) && !virtualPath.StartsWith("/");

            var path = RootPath;
            if (isAppendSeparator)
                path += "/";

            virtualPath = virtualPath.Insert(0, path);
            return base.MapPath(virtualPath);
        }

        #endregion Overrides of VirtualPathProviderBase
    }
}