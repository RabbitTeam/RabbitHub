using Rabbit.Web.Mvc.DisplayManagement;
using Rabbit.Web.Mvc.UI.Admin;
using Rabbit.Web.Mvc.Works;
using Rabbit.Web.UI.Navigation;
using Rabbit.Web.Works;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.UI.Navigation
{
    internal sealed class MenuFilter : Mvc.Filters.IFilterProvider, IResultFilter
    {
        private readonly INavigationManager _navigationManager;
        private readonly IWebWorkContextAccessor _workContextAccessor;
        private readonly dynamic _shapeFactory;

        public MenuFilter(INavigationManager navigationManager,
            IWebWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory)
        {
            _navigationManager = navigationManager;
            _workContextAccessor = workContextAccessor;
            _shapeFactory = shapeFactory;
        }

        #region Implementation of IResultFilter

        /// <summary>
        /// 在操作结果执行之前调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //应该只运行在一个完整的视图渲染结果，并且确保一个请求只被加载一次。
            if (!(filterContext.Result is ViewResult) || filterContext.HttpContext.Items.Contains("MenuRendered"))
            {
                return;
            }
            filterContext.HttpContext.Items["MenuRendered"] = null;
            var workContext = _workContextAccessor.GetWorkContext(filterContext).AsMvcWorkContext();
            if (workContext == null)
                return;

            const string menuName = "admin";
            if (!AdminFilter.IsApplied(filterContext.RequestContext))
            {
                return;
            }

            var menuItems = (filterContext.HttpContext.Items["AdminMenuList"] as MenuItem[]) ??
                _navigationManager.BuildMenu(menuName).ToArray();

            //添加查询字符串参数
            var routeData = new RouteValueDictionary(filterContext.RouteData.Values);
            var queryString = workContext.HttpContext.Request.QueryString;
            if (queryString != null)
            {
                foreach (var key in from string key in queryString.Keys where key != null && !routeData.ContainsKey(key) let value = queryString[key] select key)
                {
                    routeData[key] = queryString[key];
                }
            }

            //设置当前选择的路径
            var selectedPath = NavigationHelper.SetSelectedPath(menuItems, workContext.HttpContext.Request, routeData);

            //填充主导航
            var menuShape = _shapeFactory.Menu().MenuName(menuName);
            NavigationHelper.PopulateMenu(_shapeFactory, menuShape, menuShape, menuItems);

            //添加任何知道图像集到主导航
            var menuImageSets = _navigationManager.BuildImageSets(menuName);
            var imageSets = menuImageSets as string[] ?? menuImageSets.ToArray();
            if (imageSets.Any())
                menuShape.ImageSets(imageSets);

            workContext.Layout.Navigation.Add(menuShape);

            //填充本地导航。
            var localMenuShape = _shapeFactory.LocalMenu().MenuName(string.Format("local_{0}", menuName));
            NavigationHelper.PopulateLocalMenu(_shapeFactory, localMenuShape, localMenuShape, selectedPath);
            workContext.Layout.LocalNavigation.Add(localMenuShape);
        }

        /// <summary>
        /// 在操作结果执行后调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        #endregion Implementation of IResultFilter
    }
}