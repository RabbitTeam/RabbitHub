using Rabbit.Kernel.Works;
using Rabbit.Web.Works;

namespace Rabbit.Web.Mvc.Works
{
    /// <summary>
    /// Web工作上下文。
    /// </summary>
    public sealed class MvcWorkContext : WebWorkContext
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的Web工作上下文。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        public MvcWorkContext(WorkContext workContext)
            : base(workContext)
        {
        }

        #endregion Constructor

        /// <summary>
        /// 对应的工作范围内的布局形状。
        /// </summary>
        public dynamic Layout
        {
            get { return GetState<dynamic>("Layout"); }
            set { SetState("Layout", value); }
        }
    }
}