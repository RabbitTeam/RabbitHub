using Rabbit.Kernel.Works;
using Rabbit.Web.Mvc.DisplayManagement.Implementation;
using Rabbit.Web.Mvc.DisplayManagement.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.DisplayManagement
{
    internal sealed class ShapeDisplay : IShapeDisplay
    {
        #region Field

        private readonly IDisplayHelperFactory _displayHelperFactory;
        private readonly HttpContextBase _httpContextBase;
        private readonly RequestContext _requestContext;
        private readonly IWorkContextAccessor _workContextAccessor;

        #endregion Field

        #region Constructor

        public ShapeDisplay(
            IDisplayHelperFactory displayHelperFactory,
            IWorkContextAccessor workContextAccessor,
            HttpContextBase httpContextBase,
            RequestContext requestContext)
        {
            _displayHelperFactory = displayHelperFactory;
            _workContextAccessor = workContextAccessor;
            _httpContextBase = httpContextBase;
            _requestContext = requestContext;
        }

        #endregion Constructor

        #region Implementation of IShapeDisplay

        /// <summary>
        ///     显示。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <returns>结果。</returns>
        public string Display(Shape shape)
        {
            return Display((object)shape);
        }

        /// <summary>
        ///     显示。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <returns>结果。</returns>
        public string Display(object shape)
        {
            var viewContext = new ViewContext
            {
                HttpContext = _httpContextBase,
                RequestContext = _requestContext
            };
            viewContext.RouteData.DataTokens["IWorkContextAccessor"] = _workContextAccessor;
            dynamic display = _displayHelperFactory.CreateHelper(viewContext, new ViewDataContainer());

            return ((DisplayHelper)display).ShapeExecute(shape).ToString();
        }

        /// <summary>
        ///     显示。
        /// </summary>
        /// <param name="shapes">形状集合。</param>
        /// <returns>结果集合。</returns>
        public IEnumerable<string> Display(IEnumerable<object> shapes)
        {
            return shapes.Select(Display).ToArray();
        }

        #endregion Implementation of IShapeDisplay

        #region Help Class

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataContainer()
            {
                ViewData = new ViewDataDictionary();
            }

            public ViewDataDictionary ViewData { get; set; }
        }

        #endregion Help Class
    }
}