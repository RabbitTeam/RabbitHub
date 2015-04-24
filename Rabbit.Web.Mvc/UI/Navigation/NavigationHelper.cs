using Rabbit.Web.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.UI.Navigation
{
    /// <summary>
    /// 导航帮助者。
    /// </summary>
    public sealed class NavigationHelper
    {
        /// <summary>
        /// 填充菜单的形状。
        /// </summary>
        /// <param name="shapeFactory">形状工厂。</param>
        /// <param name="parentShape">菜单父级形状。</param>
        /// <param name="menu">菜单形状。</param>
        /// <param name="menuItems">当前等级填充形状。</param>
        public static void PopulateMenu(dynamic shapeFactory, dynamic parentShape, dynamic menu, IEnumerable<MenuItem> menuItems)
        {
            foreach (var menuItem in menuItems)
            {
                var menuItemShape = BuildMenuItemShape(shapeFactory, parentShape, menu, menuItem);

                if (menuItem.Items != null && menuItem.Items.Any())
                {
                    PopulateMenu(shapeFactory, menuItemShape, menu, menuItem.Items);
                }

                parentShape.Add(menuItemShape, menuItem.Position);
            }
        }

        /// <summary>
        /// 构建一个菜单项形状。
        /// </summary>
        /// <param name="shapeFactory">形状工厂。</param>
        /// <param name="parentShape">菜单父级形状。</param>
        /// <param name="menu">菜单形状。</param>
        /// <param name="menuItem">构建一个菜单项形状。</param>
        /// <returns>菜单项的形状。</returns>
        public static dynamic BuildMenuItemShape(dynamic shapeFactory, dynamic parentShape, dynamic menu, MenuItem menuItem)
        {
            var menuItemShape = shapeFactory.MenuItem()
                .Text(menuItem.Text)
                .Href(menuItem.Href)
                .LocalNavigation(menuItem.LocalNavigation)
                .Selected(menuItem.Selected)
                .RouteValues(menuItem.RouteValues)
                .Item(menuItem)
                .Menu(menu)
                .Parent(parentShape)
                .Icon(menuItem.Icon);

            foreach (var className in menuItem.Classes)
                menuItemShape.Classes.Add(className);

            return menuItemShape;
        }

        /// <summary>
        /// 标识当前选择的路径中，从所选择的节点开始。
        /// </summary>
        /// <param name="menuItems">在导航菜单中的所有菜单项。</param>
        /// <param name="currentRequest">当前执行的请求。</param>
        /// <param name="currentRouteData">当前的路由数据。</param>
        /// <returns>堆栈的选择路径是最后一个节点的当前所选之一。</returns>
        public static Stack<MenuItem> SetSelectedPath(IEnumerable<MenuItem> menuItems, HttpRequestBase currentRequest, RouteValueDictionary currentRouteData)
        {
            if (menuItems == null)
                return null;

            foreach (var menuItem in menuItems)
            {
                var selectedPath = SetSelectedPath(menuItem.Items, currentRequest, currentRouteData);
                if (selectedPath != null)
                {
                    menuItem.Selected = true;
                    selectedPath.Push(menuItem);
                    return selectedPath;
                }

                var match = false;
                //如果菜单项没有路由值，比较网址
                if (currentRequest != null && menuItem.RouteValues == null)
                {
                    var applicationPath = currentRequest.ApplicationPath;
                    if (!string.IsNullOrWhiteSpace(applicationPath))
                    {
                        var requestUrl = currentRequest.Path.Replace(applicationPath, string.Empty).TrimEnd('/').ToUpperInvariant();
                        var modelUrl = menuItem.Href.Replace(applicationPath, string.Empty).TrimEnd('/').ToUpperInvariant();
                        if (requestUrl == modelUrl || (!string.IsNullOrEmpty(modelUrl) && requestUrl.StartsWith(modelUrl + "/")))
                        {
                            match = true;
                        }
                    }
                }
                else
                {
                    if (RouteMatches(menuItem.RouteValues, currentRouteData))
                    {
                        match = true;
                    }
                }

                if (!match)
                    continue;
                menuItem.Selected = true;

                selectedPath = new Stack<MenuItem>();
                selectedPath.Push(menuItem);
                return selectedPath;
            }

            return null;
        }

        /// <summary>
        /// 确定一个菜单项对应于给定的路线。
        /// </summary>
        /// <param name="itemValues">菜单项。</param>
        /// <param name="requestValues">路由数据。</param>
        /// <returns>如果该菜单项的动作对应于路由数据返回true，否则返回false。</returns>
        public static bool RouteMatches(RouteValueDictionary itemValues, RouteValueDictionary requestValues)
        {
            if (itemValues == null && requestValues == null)
            {
                return true;
            }
            if (itemValues == null || requestValues == null)
            {
                return false;
            }
            if (itemValues.Keys.Any(key => requestValues.ContainsKey(key) == false))
            {
                return false;
            }
            return itemValues.Keys.All(key => string.Equals(Convert.ToString(itemValues[key]), Convert.ToString(requestValues[key]), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 从填充的第一个非本地任务的父开始本地菜单。
        /// </summary>
        /// <param name="shapeFactory">形状工厂。</param>
        /// <param name="parentShape">菜单父级形状。</param>
        /// <param name="menu">菜单形状。</param>
        /// <param name="selectedPath">选择路径。</param>
        public static void PopulateLocalMenu(dynamic shapeFactory, dynamic parentShape, dynamic menu, Stack<MenuItem> selectedPath)
        {
            var parentMenuItem = FindParentLocalTask(selectedPath);

            if (parentMenuItem != null && parentMenuItem.Items != null && parentMenuItem.Items.Any())
            {
                PopulateLocalMenu(shapeFactory, parentShape, menu, parentMenuItem.Items);
            }
        }

        /// <summary>
        /// 填入本地菜单的形状。
        /// </summary>
        /// <param name="shapeFactory">形状工厂。</param>
        /// <param name="parentShape">菜单父级形状。</param>
        /// <param name="menu">菜单形状。</param>
        /// <param name="menuItems">当前等级填充形状。</param>
        public static void PopulateLocalMenu(dynamic shapeFactory, dynamic parentShape, dynamic menu, IEnumerable<MenuItem> menuItems)
        {
            foreach (var menuItem in menuItems)
            {
                var menuItemShape = BuildLocalMenuItemShape(shapeFactory, parentShape, menu, menuItem);

                if (menuItem.Items != null && menuItem.Items.Any())
                {
                    PopulateLocalMenu(shapeFactory, menuItemShape, menu, menuItem.Items);
                }

                parentShape.Add(menuItemShape, menuItem.Position);
            }
        }

        /// <summary>
        /// 发现在选择路径的第一级，从底部开始，这不是一个本地任务。
        /// </summary>
        /// <param name="selectedPath">选择路径堆栈。底部节点是当前选定的1。</param>
        /// <returns>在第一个节点，从底部开始，这不是一个本地任务。否则，返回null。</returns>
        public static MenuItem FindParentLocalTask(Stack<MenuItem> selectedPath)
        {
            if (selectedPath == null)
                return null;
            var parentMenuItem = selectedPath.Pop();
            if (parentMenuItem == null)
                return null;
            while (selectedPath.Count > 0)
            {
                var currentMenuItem = selectedPath.Pop();
                if (currentMenuItem.LocalNavigation)
                {
                    return parentMenuItem;
                }
                parentMenuItem = currentMenuItem;
            }

            return null;
        }

        /// <summary>
        /// 构建一个本地的菜单项形状。
        /// </summary>
        /// <param name="shapeFactory">形状工厂。</param>
        /// <param name="parentShape">菜单父级形状。</param>
        /// <param name="menu">菜单形状。</param>
        /// <param name="menuItem">构建一个菜单项形状。</param>
        /// <returns>菜单项的形状。</returns>
        public static dynamic BuildLocalMenuItemShape(dynamic shapeFactory, dynamic parentShape, dynamic menu, MenuItem menuItem)
        {
            var menuItemShape = shapeFactory.LocalMenuItem()
                .Text(menuItem.Text)
                .Href(menuItem.Href)
                .LocalNavigation(menuItem.LocalNavigation)
                .Selected(menuItem.Selected)
                .RouteValues(menuItem.RouteValues)
                .Item(menuItem)
                .Menu(menu)
                .Parent(parentShape)
                .Icon(menuItem.Icon);

            foreach (var className in menuItem.Classes)
                menuItemShape.Classes.Add(className);

            return menuItemShape;
        }
    }
}