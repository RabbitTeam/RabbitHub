using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Services;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Rabbit.Kernel.FileSystems.VirtualPath.Impl
{
    internal sealed class DefaultVirtualPathMonitor : IVirtualPathMonitor
    {
        #region Field

        private readonly Thunk _thunk;
        private readonly string _prefix = Guid.NewGuid().ToString("n");
        private readonly IDictionary<string, Weak<Token>> _tokens = new Dictionary<string, Weak<Token>>();
        private readonly IClock _clock;
        private readonly IVirtualPathProvider _virtualPathProvider;

        #endregion Field

        #region Constructor

        public DefaultVirtualPathMonitor(IClock clock, IVirtualPathProvider virtualPathProvider)
        {
            _clock = clock;
            _virtualPathProvider = virtualPathProvider;
            _thunk = new Thunk(this);
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IVirtualPathMonitor

        /// <summary>
        /// 检测路径是否被更改。
        /// </summary>
        /// <param name="virtualPath">需要监测的虚拟路径。</param>
        /// <returns>挥发令牌。</returns>
        public IVolatileToken WhenPathChanges(string virtualPath)
        {
            var token = BindToken(virtualPath);
            try
            {
                BindSignal(virtualPath);
            }
            catch (HttpException e)
            {
                Logger.Information(e, "监控虚拟路径 '{0}' 时发生了错误。", virtualPath);
            }
            return token;
        }

        #endregion Implementation of IVirtualPathMonitor

        #region Private Method

        private Token BindToken(string virtualPath)
        {
            lock (_tokens)
            {
                Weak<Token> weak;
                if (!_tokens.TryGetValue(virtualPath, out weak))
                {
                    weak = new Weak<Token>(new Token(virtualPath));
                    _tokens[virtualPath] = weak;
                }

                var token = weak.Target;
                if (token != null)
                    return token;
                token = new Token(virtualPath);
                weak.Target = token;

                return token;
            }
        }

        private Token DetachToken(string virtualPath)
        {
            lock (_tokens)
            {
                Weak<Token> weak;
                if (!_tokens.TryGetValue(virtualPath, out weak))
                {
                    return null;
                }
                var token = weak.Target;
                weak.Target = null;
                return token;
            }
        }

        private void BindSignal(string virtualPath)
        {
            BindSignal(virtualPath, _thunk.Signal);
        }

        private void BindSignal(string virtualPath, CacheItemRemovedCallback callback)
        {
            var key = _prefix + virtualPath;

            if (HostingEnvironment.Cache.Get(key) != null)
                return;

            CacheDependency cacheDependency;
            if (HostingEnvironment.IsHosted)
            {
                cacheDependency = HostingEnvironment.VirtualPathProvider.GetCacheDependency(
                    virtualPath,
                    new[] { virtualPath },
                    _clock.UtcNow);
            }
            else
            {
                cacheDependency = new CacheDependency(new[] { _virtualPathProvider.MapPath(virtualPath) }, new[] { virtualPath }, _clock.UtcNow);
            }

            Logger.Debug("监控虚拟路径 \"{0}\"", virtualPath);

            HostingEnvironment.Cache.Add(
                key,
                virtualPath,
                cacheDependency,
                Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable,
                callback);
        }

        private void Signal(object value, CacheItemRemovedReason reason)
        {
            var virtualPath = Convert.ToString(value);
            Logger.Debug("虚拟路径发生了变更 ({1}) '{0}'", virtualPath, reason.ToString());

            var token = DetachToken(virtualPath);
            if (token != null)
                token.IsCurrent = false;
        }

        #endregion Private Method

        #region Help Class

        internal class Token : IVolatileToken
        {
            public Token(string virtualPath)
            {
                IsCurrent = true;
                VirtualPath = virtualPath;
            }

            public bool IsCurrent { get; set; }

            public string VirtualPath { get; private set; }

            public override string ToString()
            {
                return string.Format("IsCurrent: {0}, VirtualPath: \"{1}\"", IsCurrent, VirtualPath);
            }
        }

        private class Thunk
        {
            private readonly Weak<DefaultVirtualPathMonitor> _weak;

            public Thunk(DefaultVirtualPathMonitor provider)
            {
                _weak = new Weak<DefaultVirtualPathMonitor>(provider);
            }

            public void Signal(string key, object value, CacheItemRemovedReason reason)
            {
                var provider = _weak.Target;
                if (provider != null)
                    provider.Signal(value, reason);
            }
        }

        #endregion Help Class
    }
}