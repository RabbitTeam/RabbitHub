using Rabbit.Kernel.Localization;
using System.Collections.Generic;
using System.Web.Routing;

namespace Rabbit.Web.UI.Navigation
{
    /// <summary>
    /// 菜单项。
    /// </summary>
    public class MenuItem
    {
        private IList<string> _classes = new List<string>();
        private readonly IDictionary<string, object> _attributes = new Dictionary<string, object>();

        /// <summary>
        /// 导航文本。
        /// </summary>
        public LocalizedString Text { get; set; }

        /// <summary>
        /// Url地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 菜单链接地址。
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// 导航位置。
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 是否是本地导航。
        /// </summary>
        public bool LocalNavigation { get; set; }

        /// <summary>
        /// 是否选中。
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 路由值。
        /// </summary>
        public RouteValueDictionary RouteValues { get; set; }

        /// <summary>
        /// 菜单子项。
        /// </summary>
        public IEnumerable<MenuItem> Items { get; set; }

        /// <summary>
        /// 样式名。
        /// </summary>
        public IList<string> Classes
        {
            get { return _classes; }
            set
            {
                if (value == null)
                    return;
                _classes = value;
            }
        }

        /// <summary>
        /// 图标。
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 扩展属性。
        /// </summary>
        public IDictionary<string, object> Attributes { get { return _attributes; } }

        /// <summary>
        /// 获取一个属性。
        /// </summary>
        /// <typeparam name="T">属性类型。</typeparam>
        /// <param name="name">属性名称。</param>
        /// <returns>属性值。</returns>
        public T GetAttribute<T>(string name)
        {
            return Attributes.ContainsKey(name) ? (T)Attributes[name] : default(T);
        }

        /// <summary>
        /// 设置一个属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值。</param>
        public void SetAttribute(string name, object value)
        {
            Attributes[name] = value;
        }
    }
}