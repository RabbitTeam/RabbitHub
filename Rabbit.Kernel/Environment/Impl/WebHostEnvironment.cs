using Rabbit.Kernel.Services;
using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace Rabbit.Kernel.Environment.Impl
{
    internal sealed class WebHostEnvironment : HostEnvironment
    {
        #region Field

        private const string WebConfigPath = "~/web.config";
        private const string RefreshHtmlPath = "~/refresh.html";
        private const string HostRestartPath = "~/bin/HostRestart";
        private readonly IClock _clock;

        #endregion Field

        #region Constructor

        public WebHostEnvironment(IClock clock)
        {
            _clock = clock;
        }

        #endregion Constructor

        #region Overrides of HostEnvironment

        /// <summary>
        /// 根据虚拟路径获取物理路径。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>物理路径。</returns>
        public override string MapPath(string virtualPath)
        {
            try
            {
                return HostingEnvironment.MapPath(virtualPath);
            }
            catch
            {
                return virtualPath;
            }
        }

        /// <summary>
        /// 重新启动AppDmain。
        /// </summary>
        public override void RestartAppDomain()
        {
            if (IsFullTrust)
            {
                HttpRuntime.UnloadAppDomain();
            }
            else
            {
                var success = TryWriteBinFolder() || TryWriteWebConfig();

                if (!success)
                    throw new Exception("重启AppDomain失败。");
            }

            var httpContext = HttpContext.Current;
            if (httpContext == null) return;
            if (httpContext.Request.RequestType == "GET")
            {
                httpContext.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
            }
            else
            {
                httpContext.Response.ContentType = "text/html";
                httpContext.Response.WriteFile(RefreshHtmlPath);
                httpContext.Response.End();
            }
        }

        #endregion Overrides of HostEnvironment

        #region Private Method

        private bool TryWriteWebConfig()
        {
            try
            {
                File.SetLastWriteTimeUtc(MapPath(WebConfigPath), _clock.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryWriteBinFolder()
        {
            try
            {
                var binMarker = MapPath(HostRestartPath);
                Directory.CreateDirectory(binMarker);

                using (var stream = File.CreateText(Path.Combine(binMarker, "marker.txt")))
                {
                    stream.WriteLine("Restart on '{0}'", _clock.UtcNow);
                    stream.Flush();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion Private Method
    }
}