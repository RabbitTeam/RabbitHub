using Rabbit.Web.Mvc.Utility.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.Filters
{
    internal sealed class RabbitFilterProvider : System.Web.Mvc.IFilterProvider
    {
        #region Implementation of IFilterProvider

        /// <summary>
        /// 返回一个包含服务定位器中的所有 <see cref="T:System.Web.Mvc.IFilterProvider"/> 实例的枚举器。
        /// </summary>
        /// <returns>
        /// 包含服务定位器中的所有 <see cref="T:System.Web.Mvc.IFilterProvider"/> 实例的枚举器。
        /// </returns>
        /// <param name="controllerContext">控制器上下文。</param><param name="actionDescriptor">操作描述符。</param>
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var workContext = controllerContext.GetWorkContext();
            var filterProviders = workContext.Resolve<IEnumerable<IFilterProvider>>();
            return filterProviders.Select(x => new Filter(x, FilterScope.Action, null));
        }

        #endregion Implementation of IFilterProvider
    }
}