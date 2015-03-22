using System;
using System.IO;

namespace Rabbit.Kernel.Environment.Impl
{
    internal sealed class DefaultHostEnvironment : HostEnvironment
    {
        #region Overrides of HostEnvironment

        /// <summary>
        /// 根据虚拟路径获取物理路径。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>物理路径。</returns>
        public override string MapPath(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            virtualPath = virtualPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (Path.IsPathRooted(virtualPath) && !virtualPath.StartsWith("\\"))
                return virtualPath;

            virtualPath = virtualPath.TrimStart('~', Path.DirectorySeparatorChar);

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrWhiteSpace(virtualPath))
                return baseDirectory;

            virtualPath = Path.Combine(baseDirectory, virtualPath);
            return Path.GetFullPath(virtualPath);
        }

        /// <summary>
        /// 重新启动AppDmain。
        /// </summary>
        public override void RestartAppDomain()
        {
            throw new NotSupportedException();
        }

        #endregion Overrides of HostEnvironment
    }
}