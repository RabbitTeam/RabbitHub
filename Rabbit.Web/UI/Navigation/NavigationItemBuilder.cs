using Rabbit.Kernel.Localization;
using System.Collections.Generic;
using System.Web.Routing;

namespace Rabbit.Web.UI.Navigation
{
    /// <summary>
    /// 导航项建造者。
    /// </summary>
    public sealed class NavigationItemBuilder : NavigationBuilder
    {
        private readonly MenuItem _item = new MenuItem();

        /// <summary>
        /// 菜单项。
        /// </summary>
        public MenuItem MenuItem
        {
            get { return _item; }
        }

        /// <summary>
        /// 设置导航标题。
        /// </summary>
        /// <param name="caption">导航标题。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder Caption(LocalizedString caption)
        {
            _item.Text = caption;
            return this;
        }

        /// <summary>
        /// 设置导航的位置。
        /// </summary>
        /// <param name="position">位置。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder Position(string position)
        {
            _item.Position = position;
            return this;
        }

        /// <summary>
        /// 设置导航的Url地址。
        /// </summary>
        /// <param name="url">url地址。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder Url(string url)
        {
            _item.Url = url;
            return this;
        }

        /// <summary>
        /// 添加导航样式。
        /// </summary>
        /// <param name="className">类名称。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder AddClass(string className)
        {
            if (!_item.Classes.Contains(className))
                _item.Classes.Add(className);
            return this;
        }

        /// <summary>
        /// 删除导航样式。
        /// </summary>
        /// <param name="className">类名称。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder RemoveClass(string className)
        {
            if (_item.Classes.Contains(className))
                _item.Classes.Remove(className);
            return this;
        }

        /// <summary>
        /// 设置导航是否是本地导航。
        /// </summary>
        /// <param name="value">true为是本地导航，false不是本地导航。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder LocalNavigation(bool value = true)
        {
            _item.LocalNavigation = value;
            return this;
        }

        /// <summary>
        /// 设置导航Icon。
        /// </summary>
        /// <param name="icon">icon。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder Icon(string icon)
        {
            _item.Icon = icon;
            return this;
        }

        /// <summary>
        /// 生成导航。
        /// </summary>
        /// <returns>导航集合。</returns>
        public new IEnumerable<MenuItem> Build()
        {
            _item.Items = base.Build();
            return new[] { _item };
        }

        /// <summary>
        /// 设置导航Action。
        /// </summary>
        /// <param name="values">路由值。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder Action(RouteValueDictionary values)
        {
            return values != null
                       ? Action(values["Action"] as string, values["Controller"] as string, values)
                       : Action(null, null, new RouteValueDictionary());
        }

        /// <summary>
        /// 设置导航Action。
        /// </summary>
        /// <param name="actionName">Action名称。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder Action(string actionName)
        {
            return Action(actionName, null, new RouteValueDictionary());
        }

        /// <summary>
        /// 设置导航Action。
        /// </summary>
        /// <param name="actionName">Action名称。</param>
        /// <param name="controllerName">控制器名称。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder Action(string actionName, string controllerName)
        {
            return Action(actionName, controllerName, new RouteValueDictionary());
        }

        /// <summary>
        /// 设置导航Action。
        /// </summary>
        /// <param name="actionName">Action名称。</param>
        /// <param name="controllerName">控制器名称。</param>
        /// <param name="values">其他值。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder Action(string actionName, string controllerName, object values)
        {
            return Action(actionName, controllerName, new RouteValueDictionary(values));
        }

        /// <summary>
        /// 设置导航Action。
        /// </summary>
        /// <param name="actionName">Action名称。</param>
        /// <param name="controllerName">控制器名称。</param>
        /// <param name="values">其他值。</param>
        /// <returns>导航项建造者。</returns>
        public NavigationItemBuilder Action(string actionName, string controllerName, RouteValueDictionary values)
        {
            _item.RouteValues = new RouteValueDictionary(values);
            if (!string.IsNullOrEmpty(actionName))
                _item.RouteValues["Action"] = actionName;
            if (!string.IsNullOrEmpty(controllerName))
                _item.RouteValues["Controller"] = controllerName;
            return this;
        }
    }
}