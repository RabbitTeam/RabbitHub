using System.Collections.Generic;
using System.Reflection;

namespace Rabbit.Kernel.Environment
{
    /// <summary>
    /// 一个抽象的生成管理者。
    /// </summary>
    public interface IBuildManager : IDependency
    {
        /// <summary>
        /// 获取当前所有引用的程序集。
        /// </summary>
        /// <returns>程序集集合。</returns>
        IEnumerable<Assembly> GetReferencedAssemblies();

        /// <summary>
        /// 是否引用了名称为 <paramref name="name"/> 的程序集。
        /// </summary>
        /// <param name="name">程序集名称。</param>
        /// <returns>引用了为true，否则为false。</returns>
        bool HasReferencedAssembly(string name);

        /// <summary>
        /// 获取引用程序集。
        /// </summary>
        /// <param name="name">程序集名称。</param>
        /// <returns>程序集。</returns>
        Assembly GetReferencedAssembly(string name);

        /// <summary>
        /// 获取编译程序集。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>程序集。</returns>
        Assembly GetCompiledAssembly(string virtualPath);
    }
}