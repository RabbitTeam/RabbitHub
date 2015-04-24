using System.Web.Mvc;

namespace Rabbit.Web.Mvc.DisplayManagement.Implementation
{
    /// <summary>
    /// 显示上下文。
    /// </summary>
    public sealed class DisplayContext
    {
        /// <summary>
        /// 显示助手。
        /// </summary>
        public DisplayHelper Display { get; set; }

        /// <summary>
        /// 视图上下文。
        /// </summary>
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// 视图数据容器。
        /// </summary>
        public IViewDataContainer ViewDataContainer { get; set; }

        /// <summary>
        /// 值。
        /// </summary>
        public object Value { get; set; }
    }
}