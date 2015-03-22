using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Utility.Extensions;
using System;
using System.IO;

namespace Rabbit.Kernel.FileSystems
{
    internal abstract class FolderBase : VirtualPathProviderBase, IFolder
    {
        #region Field

        private readonly IVirtualPathMonitor _virtualPathMonitor;

        #endregion Field

        #region Constructor

        protected FolderBase(IHostEnvironment hostEnvironment, IVirtualPathMonitor virtualPathMonitor)
            : base(hostEnvironment)
        {
            _virtualPathMonitor = virtualPathMonitor;
        }

        #endregion Constructor

        #region Implementation of IFolder

        /// <summary>
        /// 创建文件。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <param name="content">文件内容。</param>
        public void CreateFile(string path, string content)
        {
            CreateFile(path, stream =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                }
            });
        }

        /// <summary>
        /// 读取文件内容。
        /// </summary>
        /// <param name="virtualPath">文件虚拟路径。</param>
        /// <returns>文件内容。</returns>
        public string ReadFile(string virtualPath)
        {
            var path = MapPath(virtualPath);
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }

        /// <summary>
        /// 储存文件。
        /// </summary>
        /// <param name="sourceFileName">源文件名称。</param>
        /// <param name="destinationPath">目标文件路径。</param>
        public void StoreFile(string sourceFileName, string destinationPath)
        {
            var path = MapPath(destinationPath);

            DeleteFile(destinationPath);

            CreateDirectory(Path.GetDirectoryName(destinationPath));
            File.Copy(sourceFileName, path, true);
        }

        /// <summary>
        /// 指定路径的文件或文件夹是否发生变更。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>挥发令牌。</returns>
        public IVolatileToken WhenPathChanges(string path)
        {
            return _virtualPathMonitor.WhenPathChanges(GetVirtualPath(path));
        }

        /// <summary>
        /// 获取路径的虚拟路径。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>虚拟路径。</returns>
        public string GetVirtualPath(string path)
        {
            path = path.NotEmptyOrWhiteSpace("path").Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (path.StartsWith(RootPath))
                return path;

            if (path.StartsWith("/"))
                path = path.Remove(0, 1);
            else if (path.StartsWith("~/"))
                path = path.Remove(0, 2);

            var rootPath = RootPath;
            if (rootPath.StartsWith("~/"))
                rootPath = rootPath.Remove(0, 2);

            if (path.StartsWith(rootPath))
                return "~/" + path;

            if (path.StartsWith(ApplicationRootFolder.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
                path = path.Remove(0, ApplicationRootFolder.Length + 1);
            return path.StartsWith(rootPath) ? "~/" + path : Combine(RootPath, path);
        }

        #endregion Implementation of IFolder
    }
}