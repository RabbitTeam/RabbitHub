using Rabbit.Kernel;
using BuilderExtensions = Rabbit.Web.BuilderExtensions;

namespace Rabbit.Components.Security.Web
{
    /// <summary>
    ///     日志建设者扩展方法。
    /// </summary>
    public static class WebSecurityBuilderExtensions
    {
        /// <summary>
        ///     使用NLog。
        /// </summary>
        /// <param name="webBuilder">Web建设者。</param>
        public static BuilderExtensions.IWebBuilder EnableSecurity(this BuilderExtensions.IWebBuilder webBuilder)
        {
            webBuilder.KernelBuilder
                .RegisterExtension(typeof(IAuthorizer).Assembly)
                .RegisterExtension(typeof(IRoleService).Assembly);

            return webBuilder;
        }
    }
}