using Rabbit.Kernel.Works;
using Rabbit.Web.Mvc.DisplayManagement;
using System;

namespace Rabbit.Web.Mvc.UI.Zones
{
    internal sealed class LayoutWorkContext : IWorkContextStateProvider
    {
        private readonly IShapeFactory _shapeFactory;

        public LayoutWorkContext(IShapeFactory shapeFactory)
        {
            _shapeFactory = shapeFactory;
        }

        #region Implementation of IWorkContextStateProvider

        /// <summary>
        /// 创建一个从一个工作上下文获取服务的委托。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <param name="name">服务名称。</param>
        /// <returns>从一个工作上下文获取服务的委托。</returns>
        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name != "Layout")
                return null;
            var layout = _shapeFactory.Create("Layout", Arguments.Empty());
            return ctx => (T)layout;
        }

        #endregion Implementation of IWorkContextStateProvider
    }
}