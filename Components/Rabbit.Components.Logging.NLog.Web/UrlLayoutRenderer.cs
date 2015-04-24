using NLog;
using NLog.LayoutRenderers;
using System.Text;
using System.Web;

namespace Rabbit.Components.Logging.NLog.Web
{
    [LayoutRenderer("url")]
    internal sealed class UrlLayoutRenderer : LayoutRenderer
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
                builder.Append("Url：");
                builder.AppendLine(HttpContext.Current.Request.Url.ToString());
            }
            catch
            {
            }
        }

        #endregion Overrides of LayoutRenderer
    }
}