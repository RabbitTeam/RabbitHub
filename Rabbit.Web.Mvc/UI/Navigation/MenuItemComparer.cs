using Rabbit.Web.UI.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Web.Mvc.UI.Navigation
{
    /// <summary>
    /// 菜单项比较器。
    /// </summary>
    public sealed class MenuItemComparer : IEqualityComparer<MenuItem>
    {
        #region Implementation of IEqualityComparer<in MenuItem>

        /// <summary>
        /// 确定指定的对象是否相等。
        /// </summary>
        /// <returns>
        /// 如果指定的对象相等，则为 true；否则为 false。
        /// </returns>
        /// <param name="x">要比较的第一个类型为 <see name="T"/> 的对象。</param><param name="y">要比较的第二个类型为 <see name="T"/> 的对象。</param>
        public bool Equals(MenuItem x, MenuItem y)
        {
            var xTextHint = x.Text == null ? null : x.Text.TextHint;
            var yTextHint = y.Text == null ? null : y.Text.TextHint;
            if (!string.Equals(xTextHint, yTextHint))
            {
                return false;
            }

            if (x.RouteValues == null || y.RouteValues == null)
                return true;
            if (x.RouteValues.Keys.Any(key => y.RouteValues.ContainsKey(key) == false))
            {
                return false;
            }
            if (y.RouteValues.Keys.Any(key => x.RouteValues.ContainsKey(key) == false))
            {
                return false;
            }
            return x.RouteValues.Keys.All(key => Equals(x.RouteValues[key], y.RouteValues[key]));
        }

        /// <summary>
        /// 返回指定对象的哈希代码。
        /// </summary>
        /// <returns>
        /// 指定对象的哈希代码。
        /// </returns>
        /// <param name="obj"><see cref="T:System.Object"/>，将为其返回哈希代码。</param><exception cref="T:System.ArgumentNullException"><paramref name="obj"/> 的类型为引用类型，<paramref name="obj"/> 为 null。</exception>
        public int GetHashCode(MenuItem obj)
        {
            var hash = 0;
            if (obj.Text != null && obj.Text.TextHint != null)
            {
                hash ^= obj.Text.TextHint.GetHashCode();
            }
            return hash;
        }

        #endregion Implementation of IEqualityComparer<in MenuItem>
    }
}