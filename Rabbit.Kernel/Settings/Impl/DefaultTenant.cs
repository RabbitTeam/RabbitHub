using System;
using System.Collections.Concurrent;
using System.Globalization;

namespace Rabbit.Kernel.Settings.Impl
{
    internal class DefaultTenant : ITenant
    {
        private readonly ConcurrentDictionary<string, object> _concurrentDictionary = new ConcurrentDictionary<string, object>();

        public DefaultTenant(string tenantName)
        {
            TenantName = tenantName;
            SuperUser = "Admin";
            TenantCulture = CultureInfo.CurrentCulture.Name;
            TenantTimeZone = TimeZoneInfo.Local.Id;
        }

        #region Implementation of ITenant

        /// <summary>
        /// 租户名称。
        /// </summary>
        public string TenantName { get; private set; }

        /// <summary>
        /// 超级用户。
        /// </summary>
        public string SuperUser { get; private set; }

        /// <summary>
        /// 租户文化。
        /// </summary>
        public string TenantCulture { get; set; }

        /// <summary>
        /// 租户时区。
        /// </summary>
        public string TenantTimeZone { get; private set; }

        /// <summary>
        /// 根据Key获取自定义参数。
        /// </summary>
        /// <param name="key">参数Key。</param>
        /// <returns>参数值。</returns>
        /// <exception cref="ArgumentNullException">参数 <paramref name="key"/> 为空。</exception>
        public object this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException("key");

                object instance;
                return _concurrentDictionary.TryGetValue(key, out instance) ? instance : null;
            }
            set
            {
                _concurrentDictionary.AddOrUpdate(key, k => value, (k, v) => value);
            }
        }

        #endregion Implementation of ITenant
    }
}