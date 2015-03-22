using System;
using System.IO;
using System.Text;

namespace Rabbit.Kernel.Environment.Configuration.Impl
{
    internal sealed class ShellSettingsSerializer
    {
        #region Field

        /// <summary>
        /// 分隔符。
        /// </summary>
        public const char Separator = ':';

        /// <summary>
        /// 空值。
        /// </summary>
        public const string EmptyValue = "null";

        /// <summary>
        /// 追加时的分隔符（性能优化）。
        /// </summary>
        public const string AppendSeparator = ": ";

        #endregion Field

        #region Public Method

        /// <summary>
        /// 将文本解析成外壳设置。
        /// </summary>
        /// <param name="text">文本。</param>
        /// <returns>外壳设置信息。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> 为空或者全为字符串。</exception>
        public static ShellSettings ParseSettings(string text)
        {
            using (var settings = new StringReader(text))
            {
                var shellSettings = new ShellSettings();
                string setting;
                while ((setting = settings.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(setting))
                        continue;

                    var separatorIndex = setting.IndexOf(Separator);
                    if (separatorIndex == -1)
                        continue;

                    var key = setting.Substring(0, separatorIndex).Trim();
                    var value = setting.Substring(separatorIndex + 1).Trim();

                    //值等于空值则跳过。
                    if (value.Equals(EmptyValue, StringComparison.OrdinalIgnoreCase))
                        continue;

                    shellSettings[key] = value;
                }

                return shellSettings;
            }
        }

        public static ShellSettings ParseSettings2(string text)
        {
            using (var settings = new StringReader(text))
            {
                var shellSettings = new ShellSettings();
                string setting;
                while ((setting = settings.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(setting))
                        continue;

                    var separatorIndex = setting.IndexOf(Separator);
                    if (separatorIndex == -1)
                        continue;

                    var key = setting.Substring(0, separatorIndex).Trim();
                    var value = setting.Substring(separatorIndex + 1).Trim();

                    //值等于空值则跳过。
                    if (value.Equals(EmptyValue, StringComparison.OrdinalIgnoreCase))
                        continue;

                    shellSettings[key] = value;
                }

                return shellSettings;
            }
        }

        /// <summary>
        /// 将外壳设置信息组成文本。
        /// </summary>
        /// <param name="settings">外壳设置信息。</param>
        /// <returns>文本。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> 为null。</exception>
        public static string ComposeSettings(ShellSettings settings)
        {
            var sb = new StringBuilder();
            foreach (var key in settings.Keys)
                sb.AppendLine(key + AppendSeparator + (settings[key] ?? EmptyValue));

            return sb.ToString();
        }

        #endregion Public Method
    }
}