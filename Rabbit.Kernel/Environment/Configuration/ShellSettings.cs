using System;
using System.Collections.Generic;

namespace Rabbit.Kernel.Environment.Configuration
{
    /// <summary>
    /// 外壳设置。
    /// </summary>
    public sealed class ShellSettings
    {
        #region Field

        /// <summary>
        /// 默认租户名称。
        /// </summary>
        public const string DefaultName = "Default";

        private readonly IDictionary<string, string> _values;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的外壳设置。
        /// </summary>
        public ShellSettings()
        {
            _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            State = TenantState.Invalid;
        }

        /// <summary>
        /// 初始化一个新的外壳设置。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        public ShellSettings(ShellSettings settings)
            : this()
        {
            foreach (var item in settings.Keys)
                this[item] = settings[item];
        }

        #endregion Constructor

        /// <summary>
        /// 外壳设置信息索引器。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>值。</returns>
        public string this[string key]
        {
            get
            {
                string retVal;
                return _values.TryGetValue(key, out retVal) ? retVal : null;
            }
            set { _values[key] = value; }
        }

        /// <summary>
        /// 所有设置信息键。
        /// </summary>
        public IEnumerable<string> Keys { get { return _values.Keys; } }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name
        {
            get { return this["Name"] ?? string.Empty; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 状态。
        /// </summary>
        public TenantState State
        {
            get
            {
                var state = this["State"];
                TenantState tenantState;
                return Enum.TryParse(state, true, out tenantState) ? tenantState : TenantState.Invalid;
            }
            set
            {
                this["State"] = value.ToString();
            }
        }
    }
}