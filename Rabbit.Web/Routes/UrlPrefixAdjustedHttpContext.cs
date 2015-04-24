using Rabbit.Web.Wrappers;
using System.Web;
using System.Web.SessionState;

namespace Rabbit.Web.Routes
{
    internal sealed class UrlPrefixAdjustedHttpContext : HttpContextBaseWrapper
    {
        private readonly UrlPrefix _prefix;

        public UrlPrefixAdjustedHttpContext(HttpContextBase httpContextBase, UrlPrefix prefix)
            : base(httpContextBase)
        {
            _prefix = prefix;
        }

        public override HttpRequestBase Request
        {
            get
            {
                return new AdjustedRequest(HttpContextBase.Request, _prefix);
            }
        }

        public override void SetSessionStateBehavior(SessionStateBehavior sessionStateBehavior)
        {
            HttpContextBase.SetSessionStateBehavior(sessionStateBehavior);
        }

        private class AdjustedRequest : HttpRequestBaseWrapper
        {
            private readonly UrlPrefix _prefix;

            public AdjustedRequest(HttpRequestBase httpRequestBase, UrlPrefix prefix)
                : base(httpRequestBase)
            {
                _prefix = prefix;
            }

            public override string AppRelativeCurrentExecutionFilePath
            {
                get
                {
                    return _prefix.RemoveLeadingSegments(HttpRequestBase.AppRelativeCurrentExecutionFilePath);
                }
            }
        }
    }
}