using Autofac;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Localization;
using Rabbit.Web.Environment.Extensions;
using Rabbit.Web.Mvc.DisplayManagement;
using Rabbit.Web.Mvc.DisplayManagement.Shapes;
using Rabbit.Web.Mvc.Mvc.Html;
using Rabbit.Web.Mvc.Mvc.Spooling;
using Rabbit.Web.Mvc.Utility.Extensions;
using Rabbit.Web.Mvc.Works;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.Razor
{
    /// <summary>
    /// Web视图页抽象类。
    /// </summary>
    /// <typeparam name="TModel">视图模型。</typeparam>
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>, IViewPage
    {
        #region Field

        private Localizer _localizer = NullLocalizer.Instance;
        private object _display;
        private object _layout;

        private IDisplayHelperFactory _displayHelperFactory;
        private IShapeFactory _shapeFactory;

        #endregion Field

        #region Property

        /// <summary>
        /// 形状工厂。
        /// </summary>
        public dynamic New { get { return ShapeFactory; } }

        /// <summary>
        /// 显示助手工厂。
        /// </summary>
        public IDisplayHelperFactory DisplayHelperFactory
        {
            get
            {
                return _displayHelperFactory ?? (_displayHelperFactory = WorkContext.Resolve<IDisplayHelperFactory>());
            }
        }

        /// <summary>
        /// 形状工厂。
        /// </summary>
        public IShapeFactory ShapeFactory
        {
            get
            {
                return _shapeFactory ?? (_shapeFactory = WorkContext.Resolve<IShapeFactory>());
            }
        }

        #endregion Property

        #region Implementation of IViewPage

        /// <summary>
        /// 本地化委托。
        /// </summary>
        public Localizer T
        {
            get
            {
                //第一次使用则需要创建。
                if (_localizer != NullLocalizer.Instance)
                    return _localizer;
                //如果该模型是一个形状，得到本地化的范围从绑定源，例如，Logon.cshtml在一个主题，overriging用户/ Logon.cshtml，需要T到回退到一个在用户
                var shape = Model as IShape;
                if (shape != null && shape.Metadata.BindingSources.Count > 1)
                {
                    var localizers = shape.Metadata.BindingSources.Reverse().Select(scope => ResolveLocalizer(ViewContext, scope)).ToList();
                    _localizer = (text, args) =>
                    {
                        foreach (var hint in localizers.Select(localizer => localizer(text, args)).Where(hint => hint.Text != text))
                        {
                            return hint;
                        }

                        //没有本地化发现，返回默认值
                        return new LocalizedString(text, VirtualPath, text, args);
                    };
                }
                else
                {
                    //没有形状，使用的VirtualPath为范围
                    _localizer = ResolveLocalizer(ViewContext, VirtualPath);
                }

                return _localizer;
            }
        }

        /// <summary>
        /// 显示形状。
        /// </summary>
        public dynamic Display { get { return _display; } }

        /// <summary>
        /// 布局页。
        /// </summary>
        public new dynamic Layout { get { return _layout; } }

        /// <summary>
        /// 显示子级内容。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <returns>Html字符串。</returns>
        public IHtmlString DisplayChildren(dynamic shape)
        {
            var writer = new HtmlStringWriter();
            foreach (var item in shape)
            {
                writer.Write(Display(item));
            }
            return writer;
        }

        /// <summary>
        /// 当前工作上下文。
        /// </summary>
        public MvcWorkContext WorkContext { get; private set; }

        /// <summary>
        /// 捕获。
        /// </summary>
        /// <param name="callback">委托。</param>
        /// <returns>可释放的对象。</returns>
        public IDisposable Capture(Action<IHtmlString> callback)
        {
            return new CaptureScope(this, callback);
        }

        /// <summary>
        /// 是否存在文本。
        /// </summary>
        /// <param name="thing">对象。</param>
        /// <returns>如果是则返回true，否则返回false。</returns>
        public bool HasText(object thing)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(thing));
        }

        #endregion Implementation of IViewPage

        #region Overrides of WebViewPage<TModel>

        /// <summary>
        /// 初始化 <see cref="T:System.Web.Mvc.AjaxHelper"/>、<see cref="T:System.Web.Mvc.HtmlHelper"/> 和 <see cref="T:System.Web.Mvc.UrlHelper"/> 类。
        /// </summary>
        public override void InitHelpers()
        {
            base.InitHelpers();

            WorkContext = ViewContext.GetWorkContext().AsMvcWorkContext();

            _display = DisplayHelperFactory.CreateHelper(ViewContext, this);
            _layout = WorkContext.Layout;
        }

        private string _tenantPrefix;

        /// <summary>
        /// Builds an absolute URL from an application-relative URL by using the specified parameters.
        /// </summary>
        /// <returns>
        /// The absolute URL.
        /// </returns>
        /// <param name="path">The initial path to use in the URL.</param><param name="pathParts">Additional path information, such as folders and subfolders.</param>
        public override string Href(string path, params object[] pathParts)
        {
            if (_tenantPrefix == null)
            {
                _tenantPrefix = WorkContext.Resolve<ShellSettings>().GetRequestUrlPrefix() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(_tenantPrefix))
                return base.Href(path, pathParts);
            if (path.StartsWith("~/")
                && !path.StartsWith("~/Modules", StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith("~/Themes", StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith("~/Media", StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith("~/Core", StringComparison.OrdinalIgnoreCase))
            {
                return base.Href("~/" + _tenantPrefix + path.Substring(2), pathParts);
            }

            return base.Href(path, pathParts);
        }

        #endregion Overrides of WebViewPage<TModel>

        #region Public Method

        /// <summary>
        /// 标签建造者。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <param name="tagName">标签名称。</param>
        /// <returns>标签建造者。</returns>
        public RabbitTagBuilder Tag(dynamic shape, string tagName)
        {
            return Html.GetWorkContext().Resolve<ITagBuilderFactory>().Create(shape, tagName);
        }

        #endregion Public Method

        #region Private Method

        private static Localizer ResolveLocalizer(ControllerContext viewContext, string scope)
        {
            var workContext = viewContext.GetWorkContext();
            if (workContext == null)
            {
                return NullLocalizer.Instance;
            }
            var text = workContext.Resolve<ILifetimeScope>().Resolve<IText>(new NamedParameter("scope", scope));
            return text.Get;
        }

        #endregion Private Method

        #region Help Class

        private class CaptureScope : IDisposable
        {
            private readonly WebPageBase _viewPage;
            private readonly Action<IHtmlString> _callback;

            public CaptureScope(WebPageBase viewPage, Action<IHtmlString> callback)
            {
                _viewPage = viewPage;
                _callback = callback;
                _viewPage.OutputStack.Push(new HtmlStringWriter());
            }

            void IDisposable.Dispose()
            {
                var writer = (HtmlStringWriter)_viewPage.OutputStack.Pop();
                _callback(writer);
            }
        }

        #endregion Help Class
    }

    /// <summary>
    /// Web视图页抽象类。
    /// </summary>
    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}