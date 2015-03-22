using Rabbit.Kernel.Environment.Assemblies;
using Rabbit.Kernel.Exceptions;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;

namespace Rabbit.Kernel.Environment.Impl
{
    internal abstract class BuildManagerBase : IBuildManager
    {
        #region Field

        private readonly IAssemblyLoader _assemblyLoader;

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        protected BuildManagerBase(IAssemblyLoader assemblyLoader)
        {
            _assemblyLoader = assemblyLoader;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IBuildManager

        /// <summary>
        /// 获取当前所有引用的程序集。
        /// </summary>
        /// <returns>程序集集合。</returns>
        public abstract IEnumerable<Assembly> GetReferencedAssemblies();

        /// <summary>
        /// 是否引用了名称为 <paramref name="name"/> 的程序集。
        /// </summary>
        /// <param name="name">程序集名称。</param>
        /// <returns>引用了为true，否则为false。</returns>
        public abstract bool HasReferencedAssembly(string name);

        /// <summary>
        /// 获取引用程序集。
        /// </summary>
        /// <param name="name">程序集名称。</param>
        /// <returns>程序集。</returns>
        public Assembly GetReferencedAssembly(string name)
        {
            return !HasReferencedAssembly(name) ? null : _assemblyLoader.Load(name);
        }

        /// <summary>
        /// 获取编译程序集。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>程序集。</returns>
        public abstract Assembly GetCompiledAssembly(string virtualPath);

        #endregion Implementation of IBuildManager
    }

    internal sealed class DefaultBuildManager : BuildManagerBase
    {
        private readonly IVirtualPathProvider _virtualPathProvider;

        public DefaultBuildManager(IAssemblyLoader assemblyLoader, IVirtualPathProvider virtualPathProvider)
            : base(assemblyLoader)
        {
            _virtualPathProvider = virtualPathProvider;
        }

        #region Overrides of BuildManagerBase

        /// <summary>
        /// 获取当前所有引用的程序集。
        /// </summary>
        /// <returns>程序集集合。</returns>
        public override IEnumerable<Assembly> GetReferencedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// 是否引用了名称为 <paramref name="name"/> 的程序集。
        /// </summary>
        /// <param name="name">程序集名称。</param>
        /// <returns>引用了为true，否则为false。</returns>
        public override bool HasReferencedAssembly(string name)
        {
            var assemblyPath = "~/" + name + ".dll";
            return _virtualPathProvider.FileExists(assemblyPath);
        }

        /// <summary>
        /// 获取编译程序集。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>程序集。</returns>
        public override Assembly GetCompiledAssembly(string virtualPath)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of BuildManagerBase
    }

    internal sealed class WebBuildManager : BuildManagerBase
    {
        #region Field

        private readonly IVirtualPathProvider _virtualPathProvider;

        #endregion Field

        #region Constructor

        public WebBuildManager(IAssemblyLoader assemblyLoader, IVirtualPathProvider virtualPathProvider)
            : base(assemblyLoader)
        {
            _virtualPathProvider = virtualPathProvider;
        }

        #endregion Constructor

        #region Overrides of BuildManager

        /// <summary>
        /// 获取当前所有引用的程序集。
        /// </summary>
        /// <returns>程序集集合。</returns>
        public override IEnumerable<Assembly> GetReferencedAssemblies()
        {
            return BuildManager.GetReferencedAssemblies().OfType<Assembly>();
        }

        /// <summary>
        /// 是否引用了名称为 <paramref name="name"/> 的程序集。
        /// </summary>
        /// <param name="name">程序集名称。</param>
        /// <returns>引用了为true，否则为false。</returns>
        public override bool HasReferencedAssembly(string name)
        {
            var assemblyPath = _virtualPathProvider.Combine("~/bin", name + ".dll");
            return _virtualPathProvider.FileExists(assemblyPath);
        }

        /// <summary>
        /// 获取编译程序集。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>程序集。</returns>
        public override Assembly GetCompiledAssembly(string virtualPath)
        {
            try
            {
                return BuildManager.GetCompiledAssembly(virtualPath);
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;

                Logger.Warning(ex, "在编译程序集时发生了错误，虚拟路径：{0}。", virtualPath);
                return null;
            }
        }

        #endregion Overrides of BuildManager
    }
}