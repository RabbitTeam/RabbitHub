using Rabbit.Kernel.Environment;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rabbit.Kernel.FileSystems
{
    internal abstract class VirtualPathProviderBase : Component, IVirtualPathProviderBase
    {
        #region Field

        private readonly IHostEnvironment _hostEnvironment;
        private string _rootFolder;

        /// <summary>
        /// 应用程序根文件夹路径 MapPath("~/")
        /// </summary>
        protected string ApplicationRootFolder;

        #endregion Field

        #region Property

        /// <summary>
        /// 根文件夹路径 MapPath(RootPath)
        /// </summary>
        protected string RootFolder
        {
            get { return _rootFolder ?? (_rootFolder = _hostEnvironment.MapPath(RootPath)); }
        }

        /// <summary>
        /// 根文件夹虚拟路径 ~/ or ~/Abc
        /// </summary>
        public abstract string RootPath { get; }

        #endregion Property

        #region Constructor

        protected VirtualPathProviderBase(IHostEnvironment hostEnvironment)
        {
            ApplicationRootFolder = hostEnvironment.MapPath("~/");
            _hostEnvironment = hostEnvironment;
        }

        #endregion Constructor

        #region Implementation of IVirtualPathProvider

        /// <summary>
        /// 组合路径。
        /// </summary>
        /// <param name="paths">路径数组。</param>
        /// <returns>组合之后的路径。</returns>
        public string Combine(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            return Fix(Path.Combine(paths));
        }

        /// <summary>
        /// 获取虚拟路径的绝对路径。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>绝对路径。</returns>
        public virtual string MapPath(string virtualPath)
        {
            return _hostEnvironment.MapPath(virtualPath);
        }

        /// <summary>
        /// 判断一个虚拟路径的文件是否存在。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>虚拟路径。</returns>
        public bool FileExists(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            return File.Exists(MapPath(virtualPath));
        }

        /// <summary>
        /// 打开一个文件。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="action">对流的操作。</param>
        /// <returns>文件流。</returns>
        public void OpenFile(string virtualPath, Action<Stream> action)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");
            if (action == null)
                throw new ArgumentNullException("action");

            var path = MapPath(virtualPath);
            if (!FileExists(path))
                return;

            using (var stream = File.OpenRead(path))
                action(stream);
        }

        /// <summary>
        /// 在指定虚拟路径创建一个文件并创建一个写入流。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="action">对写入流的操作。</param>
        /// <returns>新文件的写入流。</returns>
        public void CreateText(string virtualPath, Action<StreamWriter> action)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");
            if (action == null)
                throw new ArgumentNullException("action");

            CreateFile(virtualPath, stream =>
            {
                using (var writerStream = new StreamWriter(stream))
                    action(writerStream);
            });
        }

        /// <summary>
        /// 创建一个文件。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="action">对流的操作。</param>
        /// <returns>文件流。</returns>
        public void CreateFile(string virtualPath, Action<Stream> action)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");
            if (action == null)
                throw new ArgumentNullException("action");

            var fullPath = MapPath(virtualPath);

            //是否需要创建文件夹。
            var directoryName = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            using (var stream = File.Create(fullPath))
                action(stream);
        }

        /// <summary>
        /// 获取文件最后的写入UTC时间。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>UTC时间。</returns>
        public DateTime? GetFileLastWriteTimeUtc(string virtualPath)
        {
            var path = MapPath(virtualPath.NotEmptyOrWhiteSpace("virtualPath"));

            DateTime? dateTime = null;
            if (File.Exists(path))
                dateTime = File.GetLastWriteTimeUtc(path);

            return dateTime;
        }

        /// <summary>
        /// 获取目录最后的写入UTC时间。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>UTC时间。</returns>
        public DateTime? GetDirectoryLastWriteTimeUtc(string virtualPath)
        {
            var path = MapPath(virtualPath.NotEmptyOrWhiteSpace("virtualPath"));

            DateTime? dateTime = null;
            if (Directory.Exists(path))
                dateTime = Directory.GetLastWriteTimeUtc(path);

            return dateTime;
        }

        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        public void DeleteFile(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            var destinationFileName = MapPath(virtualPath);

            //如果目标文件不存在则跳过。
            if (!File.Exists(destinationFileName))
                return;

            //删除动作。
            Action<Action<Exception>> delete = error =>
            {
                try
                {
                    File.Delete(destinationFileName);
                }
                catch (Exception e)
                {
                    if (error != null)
                        error(e);
                }
            };

            //尝试删除文件，这一次不抛出异常。
            delete(null);

            //尝试重命名目标的唯一文件名称。
            const string extension = "deleted";
            for (var i = 0; i < 100; i++)
            {
                var newExtension = (i == 0 ? extension : string.Format("{0}{1}", extension, i));
                var newFileName = Path.ChangeExtension(destinationFileName, newExtension);
                try
                {
                    File.Delete(newFileName);
                    File.Move(destinationFileName, newFileName);
                    return;
                }
                catch
                {
                }
            }

            //再一次尝试删除文件，这一次我们将抛出异常。
            delete(e =>
            {
                throw new Exception(T("不能在 \"App_Data\" 中删除文件 \"{0}\"。", destinationFileName).ToString(), e);
            });
        }

        /// <summary>
        /// 判断目录是否存在。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>如果存在则返回true，否则返回false。</returns>
        public bool DirectoryExists(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            return Directory.Exists(MapPath(virtualPath));
        }

        /// <summary>
        /// 创建一个目录。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        public void CreateDirectory(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            var path = MapPath(virtualPath);
            //目录不等于空并且不存在时创建目录。
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 获取目录名称。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>目录名称。</returns>
        public string GetDirectoryName(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            var directoryName = Path.GetDirectoryName(virtualPath);
            return directoryName == null ? null : directoryName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// 删除目录。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        public void DeleteDirectory(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            var destinationPath = MapPath(virtualPath);

            //目录不存在则跳过。
            if (!Directory.Exists(destinationPath))
                return;

            Directory.Delete(destinationPath, true);
        }

        /// <summary>
        /// 获取指定路径下的所有文件。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="includeChildren">是否包含子级目录。</param>
        /// <returns>文件路径集合。</returns>
        public IEnumerable<string> ListFiles(string virtualPath, bool includeChildren)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            var path = MapPath(virtualPath);

            if (!FileExists(path) && !DirectoryExists(path))
                return Enumerable.Empty<string>();

            return Directory.GetFiles(path, "*", includeChildren ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(p => GetVirtualPath(virtualPath, path, p)).ToArray();
        }

        /// <summary>
        /// 获取指定路径下的所有目录。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <param name="includeChildren">是否包含子级目录。</param>
        /// <returns>目录路径集合。</returns>
        public IEnumerable<string> ListDirectories(string virtualPath, bool includeChildren)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            var path = MapPath(virtualPath);
            if (!FileExists(path) && !DirectoryExists(path))
                return Enumerable.Empty<string>();

            return Directory.GetDirectories(path, "*", includeChildren ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(p => GetVirtualPath(virtualPath, path, p)).ToArray();
        }

        #endregion Implementation of IVirtualPathProvider

        #region Private Method

        private static string GetVirtualPath(string virtualPath, string rootPath, string fullPath)
        {
            return Fix(fullPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase) ? fullPath.Remove(0, rootPath.Length).Insert(0, virtualPath) : fullPath);
        }

        private static string Fix(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? path : path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        #endregion Private Method
    }
}