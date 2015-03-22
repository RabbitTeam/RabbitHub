namespace Rabbit.Kernel.Environment.Assemblies
{
    /// <summary>
    /// 一个抽象的程序集名称解析器。
    /// </summary>
    public interface IAssemblyNameResolver
    {
        /// <summary>
        /// 排序。
        /// </summary>
        int Order { get; }

        /// <summary>
        /// 解析一个程序集短名称到一个完全名称。
        /// </summary>
        /// <param name="shortName">程序集短名称。</param>
        /// <returns>程序集完全名称。</returns>
        string Resolve(string shortName);
    }
}