using Rabbit.Kernel.Localization.Services;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Works;
using System;
using System.Globalization;

namespace Rabbit.Kernel.Localization.Impl
{
    internal sealed class Text : IText
    {
        #region Field

        private readonly string _scope;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILocalizedStringManager _localizedStringManager;

        #endregion Field

        #region Constructor

        public Text(string scope, IWorkContextAccessor workContextAccessor, ILocalizedStringManager localizedStringManager)
        {
            _scope = scope;
            _workContextAccessor = workContextAccessor;
            _localizedStringManager = localizedStringManager;
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IText

        public LocalizedString Get(string textHint, params object[] args)
        {
            Logger.Debug("准备获取范围 {0} 的本地化字符串 '{1}'", _scope, textHint);

            var workContext = _workContextAccessor.GetContext();
            var currentCulture = workContext.CurrentCulture;
            var localizedFormat = _localizedStringManager.GetLocalizedString(_scope, textHint, currentCulture);

            return args.Length == 0
                ? new LocalizedString(localizedFormat, _scope, textHint, args)
                : new LocalizedString(string.Format(GetFormatProvider(currentCulture), localizedFormat, args), _scope, textHint, args);
        }

        #endregion Implementation of IText

        #region Private Method

        private static IFormatProvider GetFormatProvider(string currentCulture)
        {
            try
            {
                return CultureInfo.GetCultureInfoByIetfLanguageTag(currentCulture);
            }
            catch
            {
                return null;
            }
        }

        #endregion Private Method
    }
}