using Autofac;
using Rabbit.Web.Works;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.Mvc
{
    internal sealed class MvcModule : Module
    {
        #region Overrides of Module

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be
        ///             registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(HttpContextBaseFactory).As<HttpContextBase>().InstancePerDependency();
            builder.Register(RequestContextFactory).As<RequestContext>().InstancePerDependency();
            builder.Register(UrlHelperFactory).As<UrlHelper>().InstancePerDependency();
        }

        #endregion Overrides of Module

        private static bool IsRequestValid()
        {
            if (HttpContext.Current == null)
                return false;

            try
            {
                var req = HttpContext.Current.Request;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static HttpContextBase HttpContextBaseFactory(IComponentContext context)
        {
            if (IsRequestValid())
            {
                return new HttpContextWrapper(HttpContext.Current);
            }

            var baseUrl = new Func<string>(() => string.Empty);
            var httpContextBase = new HttpContextPlaceholder(baseUrl);
            context.Resolve<IWebWorkContextAccessor>().CreateWorkContextScope(httpContextBase);
            return httpContextBase;
        }

        private static RequestContext RequestContextFactory(IComponentContext context)
        {
            var httpContextAccessor = context.Resolve<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.Current();
            if (httpContext != null)
            {
                var mvcHandler = httpContext.Handler as MvcHandler;
                if (mvcHandler != null)
                {
                    return mvcHandler.RequestContext;
                }

                var hasRequestContext = httpContext.Handler as IHasRequestContext;
                if (hasRequestContext != null)
                {
                    if (hasRequestContext.RequestContext != null)
                        return hasRequestContext.RequestContext;
                }
            }
            else
            {
                httpContext = HttpContextBaseFactory(context);
            }

            return new RequestContext(httpContext, new RouteData());
        }

        private static UrlHelper UrlHelperFactory(IComponentContext context)
        {
            return new UrlHelper(context.Resolve<RequestContext>(), context.Resolve<RouteCollection>());
        }

        private class HttpContextPlaceholder : HttpContextBase
        {
            private readonly Lazy<string> _baseUrl;
            private readonly IDictionary _items = new Dictionary<object, object>();

            public HttpContextPlaceholder(Func<string> baseUrl)
            {
                _baseUrl = new Lazy<string>(baseUrl);
            }

            public override HttpRequestBase Request
            {
                get { return new HttpRequestPlaceholder(new Uri(_baseUrl.Value)); }
            }

            public override IHttpHandler Handler { get; set; }

            public override HttpResponseBase Response
            {
                get { return new HttpResponsePlaceholder(); }
            }

            public override IDictionary Items
            {
                get { return _items; }
            }

            /*            public override PageInstrumentationService PageInstrumentation
                        {
                            get { return new PageInstrumentationService(); }
                        }*/

            public override Cache Cache
            {
                get { return HttpRuntime.Cache; }
            }
        }

        private class HttpResponsePlaceholder : HttpResponseBase
        {
            public override string ApplyAppPathModifier(string virtualPath)
            {
                return virtualPath;
            }
        }

        private class HttpRequestPlaceholder : HttpRequestBase
        {
            private readonly Uri _uri;

            public HttpRequestPlaceholder(Uri uri)
            {
                _uri = uri;
            }

            public override bool IsAuthenticated
            {
                get { return false; }
            }

            public override NameValueCollection Form
            {
                get
                {
                    return new NameValueCollection();
                }
            }

            public override Uri Url
            {
                get
                {
                    return _uri;
                }
            }

            public override NameValueCollection Headers
            {
                get
                {
                    return new NameValueCollection { { "Host", _uri.Authority } };
                }
            }

            public override string AppRelativeCurrentExecutionFilePath
            {
                get
                {
                    return "~/";
                }
            }

            public override string ApplicationPath
            {
                get
                {
                    return _uri.LocalPath;
                }
            }

            public override NameValueCollection ServerVariables
            {
                get
                {
                    return new NameValueCollection {
                        { "SERVER_PORT", _uri.Port.ToString(CultureInfo.InvariantCulture) },
                        { "HTTP_HOST", _uri.Authority.ToString(CultureInfo.InvariantCulture) },
                    };
                }
            }

            public override bool IsLocal
            {
                get { return true; }
            }
        }
    }
}