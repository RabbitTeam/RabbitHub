using System.Web;

namespace Rabbit.Web
{
    /// <summary>
    /// 一个抽象的Http上下文访问器。
    /// </summary>
    public interface IHttpContextAccessor
    {
        /// <summary>
        /// 当前Http上下文。
        /// </summary>
        /// <returns>Http上下文。</returns>
        HttpContextBase Current();

        /// <summary>
        /// 设置Http上下文。
        /// </summary>
        /// <param name="stub">存根。</param>
        void Set(HttpContextBase stub);
    }
}