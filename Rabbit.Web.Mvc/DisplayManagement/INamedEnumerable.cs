using System.Collections.Generic;

namespace Rabbit.Web.Mvc.DisplayManagement
{
    /// <summary>
    /// 一个抽象的命名枚举。
    /// </summary>
    /// <typeparam name="T">类型。</typeparam>
    public interface INamedEnumerable<T> : IEnumerable<T>
    {
        /// <summary>
        /// 阵地。
        /// </summary>
        IEnumerable<T> Positional { get; }

        /// <summary>
        /// 命名。
        /// </summary>
        IDictionary<string, T> Named { get; }
    }
}