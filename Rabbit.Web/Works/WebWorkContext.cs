using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Works;
using System.Web;

namespace Rabbit.Web.Works
{
    /// <summary>
    /// Web工作上下文。
    /// </summary>
    public class WebWorkContext : WorkContext
    {
        #region Field

        private readonly WorkContext _workContext;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的Web工作上下文。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        public WebWorkContext(WorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion Constructor

        #region Overrides of WorkContext

        /// <summary>
        /// 解析一个服务。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>服务实例。</returns>
        public override T Resolve<T>()
        {
            return _workContext.Resolve<T>();
        }

        /// <summary>
        /// 尝试解析一个服务。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <param name="service">服务实例。</param>
        /// <returns>如果解析成功则返回true，否则返回false。</returns>
        public override bool TryResolve<T>(out T service)
        {
            return _workContext.TryResolve(out service);
        }

        /// <summary>
        /// 获取一个状态。
        /// </summary>
        /// <typeparam name="T">状态类型。</typeparam>
        /// <param name="name">状态名称。</param>
        /// <returns>状态实例。</returns>
        public override T GetState<T>(string name)
        {
            return _workContext.GetState<T>(name);
        }

        /// <summary>
        /// 设置一个状态。
        /// </summary>
        /// <typeparam name="T">状态类型。</typeparam>
        /// <param name="name">状态名称。</param>
        /// <param name="value">状态值。</param>
        public override void SetState<T>(string name, T value)
        {
            _workContext.SetState(name, value);
        }

        #endregion Overrides of WorkContext

        #region Property

        /// <summary>
        /// HTTP上下文对应的工作环境。
        /// </summary>
        public HttpContextBase HttpContext
        {
            get { return GetState<HttpContextBase>("HttpContext"); }
            set { SetState("HttpContext", value); }
        }

        /// <summary>
        /// 当前上下文正在使用的主题。
        /// </summary>
        public ExtensionDescriptorEntry CurrentTheme
        {
            get { return GetState<ExtensionDescriptorEntry>("CurrentTheme"); }
            set { SetState("CurrentTheme", value); }
        }

        #endregion Property
    }
}