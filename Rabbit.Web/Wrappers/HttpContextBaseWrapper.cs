using System;
using System.Collections;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Profile;

namespace Rabbit.Web.Wrappers
{
    internal abstract class HttpContextBaseWrapper : HttpContextBase
    {
        protected readonly HttpContextBase HttpContextBase;

        protected HttpContextBaseWrapper(HttpContextBase httpContextBase)
        {
            HttpContextBase = httpContextBase;
        }

        public override void AddError(Exception errorInfo)
        {
            HttpContextBase.AddError(errorInfo);
        }

        public override void ClearError()
        {
            HttpContextBase.ClearError();
        }

        public override object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            return HttpContextBase.GetGlobalResourceObject(classKey, resourceKey);
        }

        public override object GetGlobalResourceObject(string classKey, string resourceKey, CultureInfo culture)
        {
            return HttpContextBase.GetGlobalResourceObject(classKey, resourceKey, culture);
        }

        public override object GetLocalResourceObject(string virtualPath, string resourceKey)
        {
            return HttpContextBase.GetLocalResourceObject(virtualPath, resourceKey);
        }

        public override object GetLocalResourceObject(string virtualPath, string resourceKey, CultureInfo culture)
        {
            return HttpContextBase.GetLocalResourceObject(virtualPath, resourceKey, culture);
        }

        public override object GetSection(string sectionName)
        {
            return HttpContextBase.GetSection(sectionName);
        }

        public override object GetService(Type serviceType)
        {
            return ((IServiceProvider)HttpContextBase).GetService(serviceType);
        }

        public override void RewritePath(string path)
        {
            HttpContextBase.RewritePath(path);
        }

        public override void RewritePath(string path, bool rebaseClientPath)
        {
            HttpContextBase.RewritePath(path, rebaseClientPath);
        }

        public override void RewritePath(string filePath, string pathInfo, string queryString)
        {
            HttpContextBase.RewritePath(filePath, pathInfo, queryString);
        }

        public override void RewritePath(string filePath, string pathInfo, string queryString, bool setClientFilePath)
        {
            HttpContextBase.RewritePath(filePath, pathInfo, queryString, setClientFilePath);
        }

        public override Exception[] AllErrors
        {
            get
            {
                return HttpContextBase.AllErrors;
            }
        }

        public override HttpApplicationStateBase Application
        {
            get
            {
                return HttpContextBase.Application;
            }
        }

        public override HttpApplication ApplicationInstance
        {
            get
            {
                return HttpContextBase.ApplicationInstance;
            }
            set
            {
                HttpContextBase.ApplicationInstance = value;
            }
        }

        public override Cache Cache
        {
            get
            {
                return HttpContextBase.Cache;
            }
        }

        public override IHttpHandler CurrentHandler
        {
            get
            {
                return HttpContextBase.CurrentHandler;
            }
        }

        public override RequestNotification CurrentNotification
        {
            get
            {
                return HttpContextBase.CurrentNotification;
            }
        }

        public override Exception Error
        {
            get
            {
                return HttpContextBase.Error;
            }
        }

        public override IHttpHandler Handler
        {
            get
            {
                return HttpContextBase.Handler;
            }
            set
            {
                HttpContextBase.Handler = value;
            }
        }

        public override bool IsCustomErrorEnabled
        {
            get
            {
                return HttpContextBase.IsCustomErrorEnabled;
            }
        }

        public override bool IsDebuggingEnabled
        {
            get
            {
                return HttpContextBase.IsDebuggingEnabled;
            }
        }

        public override bool IsPostNotification
        {
            get
            {
                return HttpContextBase.IsDebuggingEnabled;
            }
        }

        public override IDictionary Items
        {
            get
            {
                return HttpContextBase.Items;
            }
        }

        public override IHttpHandler PreviousHandler
        {
            get
            {
                return HttpContextBase.PreviousHandler;
            }
        }

        public override ProfileBase Profile
        {
            get
            {
                return HttpContextBase.Profile;
            }
        }

        public override HttpRequestBase Request
        {
            get
            {
                return HttpContextBase.Request;
            }
        }

        public override HttpResponseBase Response
        {
            get
            {
                return HttpContextBase.Response;
            }
        }

        public override HttpServerUtilityBase Server
        {
            get
            {
                return HttpContextBase.Server;
            }
        }

        public override HttpSessionStateBase Session
        {
            get
            {
                return HttpContextBase.Session;
            }
        }

        public override bool SkipAuthorization
        {
            get
            {
                return HttpContextBase.SkipAuthorization;
            }
            set
            {
                HttpContextBase.SkipAuthorization = value;
            }
        }

        public override DateTime Timestamp
        {
            get
            {
                return HttpContextBase.Timestamp;
            }
        }

        public override TraceContext Trace
        {
            get
            {
                return HttpContextBase.Trace;
            }
        }

        public override IPrincipal User
        {
            get
            {
                return HttpContextBase.User;
            }
            set
            {
                HttpContextBase.User = value;
            }
        }
    }
}