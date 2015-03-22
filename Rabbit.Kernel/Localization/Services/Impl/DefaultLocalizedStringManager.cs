using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Rabbit.Kernel.Localization.Services.Impl
{
    internal sealed class DefaultLocalizedStringManager : ILocalizedStringManager
    {
        #region Field

        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IVirtualPathMonitor _virtualPathMonitor;
        private readonly IExtensionManager _extensionManager;
        private readonly ICacheManager _cacheManager;
        private readonly ShellSettings _shellSettings;
        private readonly ISignals _signals;
        private const string ModulesLocalizationFilePathFormat = "~/Modules/{0}/App_Data/Localization/{1}/rabbit.module.po";
        private const string RootLocalizationFilePathFormat = "~/App_Data/Localization/{0}/rabbit.root.po";
        private const string TenantLocalizationFilePathFormat = "~/App_Data/Tenants/{0}/Localization/{1}/rabbit.po";

        #endregion Field

        #region Constructor

        public DefaultLocalizedStringManager(IVirtualPathProvider virtualPathProvider, IVirtualPathMonitor virtualPathMonitor, IExtensionManager extensionManager, ICacheManager cacheManager, ShellSettings shellSettings, ISignals signals)
        {
            _virtualPathProvider = virtualPathProvider;
            _virtualPathMonitor = virtualPathMonitor;
            _extensionManager = extensionManager;
            _cacheManager = cacheManager;
            _shellSettings = shellSettings;
            _signals = signals;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        public bool DisableMonitoring { get; set; }

        #endregion Property

        #region Implementation of ILocalizedStringManager

        /// <summary>
        /// 获取本地化字符串。
        /// </summary>
        /// <param name="scope">作用范围。</param>
        /// <param name="text">文本。</param>
        /// <param name="cultureName">文化名称。</param>
        /// <returns>本地化字符串。</returns>
        public string GetLocalizedString(string scope, string text, string cultureName)
        {
            var culture = LoadCulture(cultureName);

            var scopedKey = (scope + "|" + text).ToLowerInvariant();
            if (culture.Translations.ContainsKey(scopedKey))
            {
                return culture.Translations[scopedKey];
            }

            var genericKey = ("|" + text).ToLowerInvariant();
            return culture.Translations.ContainsKey(genericKey) ? culture.Translations[genericKey] : GetParentTranslation(scope, text, cultureName);
        }

        #endregion Implementation of ILocalizedStringManager

        #region Private Method

        private string GetParentTranslation(string scope, string text, string cultureName)
        {
            var scopedKey = (scope + "|" + text).ToLowerInvariant();
            var genericKey = ("|" + text).ToLowerInvariant();
            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
                var parentCultureInfo = cultureInfo.Parent;
                if (parentCultureInfo.IsNeutralCulture)
                {
                    var culture = LoadCulture(parentCultureInfo.Name);
                    if (culture.Translations.ContainsKey(scopedKey))
                    {
                        return culture.Translations[scopedKey];
                    }
                    if (culture.Translations.ContainsKey(genericKey))
                    {
                        return culture.Translations[genericKey];
                    }
                    return text;
                }
            }
            catch (CultureNotFoundException) { }

            return text;
        }

        private CultureDictionary LoadCulture(string culture)
        {
            return _cacheManager.Get(culture, ctx =>
            {
                ctx.Monitor(_signals.When("culturesChanged"));
                return new CultureDictionary
                {
                    CultureName = culture,
                    Translations = LoadTranslationsForCulture(culture, ctx)
                };
            });
        }

        // 从多个地点进行合并:
        // "~/Modules/<module_name>/App_Data/Localization/<culture_name>/rabbit.module.po";
        // "~/App_Data/Localization/<culture_name>/rabbit.root.po";
        // "~/App_Data/Tenants/<tenant_name>/Localization/<culture_name>/rabbit.po";
        private IDictionary<string, string> LoadTranslationsForCulture(string culture, AcquireContext<string> context)
        {
            IDictionary<string, string> translations = new Dictionary<string, string>();
            string text;
            foreach (var module in _extensionManager.AvailableExtensions())
            {
                if (module.ExtensionType != "Module")
                    continue;

                var modulePath = string.Format(ModulesLocalizationFilePathFormat, module.Id, culture);
                text = ReadFile(modulePath);

                if (text == null)
                    continue;

                ParseLocalizationStream(text, translations, true);

                if (DisableMonitoring)
                    continue;

                Logger.Debug("监控虚拟路径 \"{0}\"", modulePath);
                context.Monitor(_virtualPathMonitor.WhenPathChanges(modulePath));
            }

            var rootPath = string.Format(RootLocalizationFilePathFormat, culture);
            text = ReadFile(rootPath);
            if (text != null)
            {
                ParseLocalizationStream(text, translations, true);
                if (!DisableMonitoring)
                {
                    Logger.Debug("监控虚拟路径 \"{0}\"", rootPath);
                    context.Monitor(_virtualPathMonitor.WhenPathChanges(rootPath));
                }
            }

            var tenantPath = string.Format(TenantLocalizationFilePathFormat, _shellSettings.Name, culture);
            text = ReadFile(tenantPath);

            if (text == null)
                return translations;

            ParseLocalizationStream(text, translations, true);

            if (DisableMonitoring)
                return translations;

            Logger.Debug("监控虚拟路径 \"{0}\"", tenantPath);
            context.Monitor(_virtualPathMonitor.WhenPathChanges(tenantPath));

            return translations;
        }

        private static readonly Dictionary<char, char> EscapeTranslations = new Dictionary<char, char> {
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' }
        };

        private static string Unescape(string str)
        {
            StringBuilder sb = null;
            var escaped = false;
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (escaped)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder(str.Length);
                        if (i > 1)
                        {
                            sb.Append(str.Substring(0, i - 1));
                        }
                    }
                    char unescaped;
                    sb.Append(EscapeTranslations.TryGetValue(c, out unescaped) ? unescaped : c);
                    escaped = false;
                }
                else
                {
                    if (c == '\\')
                    {
                        escaped = true;
                    }
                    else if (sb != null)
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb == null ? str : sb.ToString();
        }

        private static void ParseLocalizationStream(string text, IDictionary<string, string> translations, bool merge)
        {
            var reader = new StringReader(text);
            string poLine, scope;
            var id = scope = string.Empty;
            while ((poLine = reader.ReadLine()) != null)
            {
                if (poLine.StartsWith("#:"))
                {
                    scope = ParseScope(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgctxt"))
                {
                    scope = ParseContext(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgid"))
                {
                    id = ParseId(poLine);
                    continue;
                }

                if (!poLine.StartsWith("msgstr"))
                    continue;
                var translation = ParseTranslation(poLine);
                //忽略不完整的本地化（空msgid或msgstr）
                if (!String.IsNullOrWhiteSpace(id) && !String.IsNullOrWhiteSpace(translation))
                {
                    var scopedKey = (scope + "|" + id).ToLowerInvariant();
                    if (!translations.ContainsKey(scopedKey))
                    {
                        translations.Add(scopedKey, translation);
                    }
                    else
                    {
                        if (merge)
                        {
                            translations[scopedKey] = translation;
                        }
                    }
                }
                id = scope = string.Empty;
            }
        }

        private static string ParseTranslation(string poLine)
        {
            return Unescape(poLine.Substring(6).Trim().Trim('"'));
        }

        private static string ParseId(string poLine)
        {
            return Unescape(poLine.Substring(5).Trim().Trim('"'));
        }

        private static string ParseScope(string poLine)
        {
            return Unescape(poLine.Substring(2).Trim().Trim('"'));
        }

        private static string ParseContext(string poLine)
        {
            return Unescape(poLine.Substring(7).Trim().Trim('"'));
        }

        private class CultureDictionary
        {
            public string CultureName { get; set; }

            public IDictionary<string, string> Translations { get; set; }
        }

        private string ReadFile(string path)
        {
            path = _virtualPathProvider.MapPath(path);
            return !File.Exists(path) ? null : File.ReadAllText(path);
        }

        #endregion Private Method
    }
}