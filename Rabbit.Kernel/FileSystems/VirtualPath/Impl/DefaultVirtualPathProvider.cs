using Rabbit.Kernel.Environment;

namespace Rabbit.Kernel.FileSystems.VirtualPath.Impl
{
    internal sealed class DefaultVirtualPathProvider : VirtualPathProviderBase
    {
        #region Constructor

        public DefaultVirtualPathProvider(IHostEnvironment hostEnvironment)
            : base(hostEnvironment)
        {
        }

        #endregion Constructor

        #region Overrides of VirtualPathProviderBase

        /// <summary>
        /// 根文件夹虚拟路径 ~/ or ~/Abc
        /// </summary>
        public override string RootPath
        {
            get { return "~/"; }
        }

        #endregion Overrides of VirtualPathProviderBase
    }
}