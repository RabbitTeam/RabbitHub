using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Web.Environment.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Rabbit.Web.Routes.Impl
{
    internal sealed class DefaultRunningShellTable : IRunningShellTable, IDisposable
    {
        #region Field

        private IEnumerable<ShellSettings> _shells = Enumerable.Empty<ShellSettings>();
        private IDictionary<string, IEnumerable<ShellSettings>> _shellsByHost;
        private readonly ConcurrentDictionary<string, ShellSettings> _shellsByHostAndPrefix = new ConcurrentDictionary<string, ShellSettings>(StringComparer.OrdinalIgnoreCase);
        private ShellSettings _fallback;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        #endregion Field

        #region Implementation of IRunningShellTable

        /// <summary>
        /// 添加。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        public void Add(ShellSettings settings)
        {
            _lock.EnterWriteLock();
            try
            {
                _shells = _shells
                    .Where(s => s.Name != settings.Name)
                    .Concat(new[] { settings })
                    .ToArray();

                Organize();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        public void Remove(ShellSettings settings)
        {
            _lock.EnterWriteLock();
            try
            {
                _shells = _shells
                    .Where(s => s.Name != settings.Name)
                    .ToArray();

                Organize();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        public void Update(ShellSettings settings)
        {
            _lock.EnterWriteLock();
            try
            {
                _shells = _shells
                    .Where(s => s.Name != settings.Name)
                    .ToArray();

                _shells = _shells
                    .Concat(new[] { settings })
                    .ToArray();

                Organize();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 匹配。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        /// <returns>外壳设置。</returns>
        public ShellSettings Match(HttpContextBase httpContext)
        {
            //使用主机头，以防止原始请求的变更代理
            try
            {
                var httpRequest = httpContext.Request;
                if (httpRequest == null)
                {
                    return null;
                }

                var host = httpRequest.Headers["Host"];
                var appRelativeCurrentExecutionFilePath = httpRequest.AppRelativeCurrentExecutionFilePath;

                return Match(host ?? string.Empty, appRelativeCurrentExecutionFilePath);
            }
            catch (HttpException)
            {
                //可能发生在云服务，原因未知。
                return null;
            }
        }

        /// <summary>
        /// 匹配。
        /// </summary>
        /// <param name="host">主机名称。</param>
        /// <param name="appRelativeCurrentExecutionFilePath">应用程序相对当前执行文件路径。</param>
        /// <returns>外壳设置，</returns>
        public ShellSettings Match(string host, string appRelativeCurrentExecutionFilePath)
        {
            _lock.EnterReadLock();
            try
            {
                if (_shellsByHost == null)
                {
                    return null;
                }

                //优化的路径，当只有一个租户（默认），配置有没有自定义的主机
                if (!_shellsByHost.Any() && _fallback != null)
                {
                    return _fallback;
                }

                //从主机中删除该端口
                var hostLength = host.IndexOf(':');
                if (hostLength != -1)
                {
                    host = host.Substring(0, hostLength);
                }

                var hostAndPrefix = host + "/" + appRelativeCurrentExecutionFilePath.Split('/')[1];

                return _shellsByHostAndPrefix.GetOrAdd(hostAndPrefix, key =>
                {
                    //通过主机过滤外壳
                    IEnumerable<ShellSettings> shells;

                    if (!_shellsByHost.TryGetValue(host, out shells))
                    {
                        if (!_shellsByHost.TryGetValue(string.Empty, out shells))
                        {
                            //没有具体的匹配，然后寻找起始映射
                            var subHostKey = _shellsByHost.Keys.FirstOrDefault(x =>
                                x.StartsWith("*.") && host.EndsWith(x.Substring(2)));

                            if (subHostKey == null)
                            {
                                return _fallback;
                            }

                            shells = _shellsByHost[subHostKey];
                        }
                    }

                    //寻找一个请求的URL前缀匹配
                    var mostQualifiedMatch = shells.FirstOrDefault(settings =>
                    {
                        if (settings.State == TenantState.Disabled)
                        {
                            return false;
                        }

                        return string.IsNullOrWhiteSpace(settings.GetRequestUrlPrefix()) || key.Equals(host + "/" + settings.GetRequestUrlPrefix(), StringComparison.OrdinalIgnoreCase);
                    });

                    return mostQualifiedMatch ?? _fallback;
                });
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion Implementation of IRunningShellTable

        #region Private Method

        private void Organize()
        {
            var qualified =
                _shells.Where(x => !string.IsNullOrEmpty(x.GetRequestUrlHost()) || !string.IsNullOrEmpty(x.GetRequestUrlPrefix()));

            var unqualified = _shells
                .Where(x => string.IsNullOrEmpty(x.GetRequestUrlHost()) && string.IsNullOrEmpty(x.GetRequestUrlPrefix()))
                .ToList();

            _shellsByHost = qualified
                .SelectMany(s => s.GetRequestUrlHost() == null || s.GetRequestUrlHost().IndexOf(',') == -1 ? new[] { s } :
                    s.GetRequestUrlHost().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(h =>
                     {
                         var settings = new ShellSettings(s);
                         settings.SetRequestUrlHost(h);
                         return settings;
                     }))
                .GroupBy(s => s.GetRequestUrlHost() ?? string.Empty)
                .OrderByDescending(g => g.Key.Length)
                .ToDictionary(x => x.Key, x => x.AsEnumerable(), StringComparer.OrdinalIgnoreCase);

            if (unqualified.Count() == 1)
            {
                _fallback = unqualified.Single();
            }
            else if (unqualified.Any())
            {
                _fallback = unqualified.SingleOrDefault(x => x.Name == ShellSettings.DefaultName);
            }
            else
            {
                _fallback = null;
            }

            _shellsByHostAndPrefix.Clear();
        }

        #endregion Private Method

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            _lock.Dispose();
        }

        #endregion Implementation of IDisposable
    }
}