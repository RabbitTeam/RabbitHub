using NLog;
using NLog.LayoutRenderers;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Web.Works;
using System.Text;
using System.Web;

namespace Rabbit.Components.Logging.NLog.Web
{
    [LayoutRenderer("tenant")]
    internal sealed class TenantLayoutRenderer : LayoutRenderer
    {
        #region Overrides of LayoutRenderer

        /// <summary>
        /// Renders the specified environmental information and appends it to the specified <see cref="T:System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="T:System.Text.StringBuilder"/> to append the rendered data to.</param><param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            try
            {
                if (HttpContext.Current == null)
                    return;

                var context = new HttpContextWrapper(HttpContext.Current);

                if (context.Request.RequestContext == null)
                    return;

                var work = context.Request.RequestContext.GetWorkContext();
                if (work == null)
                    return;

                ShellSettings settings;
                if (!work.TryResolve(out settings))
                    return;

                builder.Append("Tenant：");
                builder.AppendLine(settings.Name);
            }
            catch
            {
            }
        }

        #endregion Overrides of LayoutRenderer
    }
}