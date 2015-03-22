using System;

namespace Rabbit.Kernel
{
    /// <summary>
    /// 版本范围。
    /// </summary>
    public sealed class VersionRange
    {
        #region Field

        private readonly Version _minVersion;
        private readonly Version _maxVersion;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个版本范围。
        /// </summary>
        /// <param name="minVersion">最小版本。</param>
        /// <param name="maxVersion">最大版本。</param>
        public VersionRange(Version minVersion, Version maxVersion)
        {
            if (minVersion == null)
                throw new ArgumentNullException("minVersion");
            if (maxVersion == null)
                throw new ArgumentNullException("maxVersion");

            _minVersion = minVersion;
            _maxVersion = maxVersion;
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 最小版本。
        /// </summary>
        public Version MinVersion { get { return _minVersion; } }

        /// <summary>
        /// 最大版本。
        /// </summary>
        public Version MaxVersion { get { return _maxVersion; } }

        #endregion Property

        #region Public Method

        /// <summary>
        /// 比较版本是否在版本范围之内。
        /// </summary>
        /// <param name="version">需要比较的版本。</param>
        /// <returns>如果在版本范围之内则返回true，否则返回false。</returns>
        public bool IsInRange(Version version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            if (_minVersion == _maxVersion)
                return version == _minVersion;
            return version < _maxVersion && version >= _minVersion;
        }

        #endregion Public Method

        #region Overrides of ValueType

        /// <summary>
        /// 返回格式为：{0}-{1}的版本范围字符串，如果最大和最小版本都为null则返回空字符串，如果最大或者最小版本只有一个为null则返回单个版本字符串。
        /// </summary>
        /// <returns>
        /// 版本字符串信息。
        /// </returns>
        public override string ToString()
        {
            return ToString("{0}-{1}");
        }

        /// <summary>
        /// 返回格式为：<paramref name="format"/> 的版本范围字符串，如果最大和最小版本都为null则返回空字符串，如果最大或者最小版本只有一个为null则返回单个版本字符串。
        /// </summary>
        /// <param name="format">格式。</param>
        /// <returns>指定格式的版本字符串信息。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> 为空。</exception>
        public string ToString(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentNullException("format");

            if (MinVersion != null && MaxVersion != null)
            {
                return string.Format(format, MinVersion, MaxVersion);
            }
            if (MinVersion == null && MaxVersion != null)
            {
                return MaxVersion.ToString();
            }
            if (MaxVersion == null && MinVersion != null)
            {
                return MinVersion.ToString();
            }
            return string.Empty;
        }

        #endregion Overrides of ValueType
    }
}