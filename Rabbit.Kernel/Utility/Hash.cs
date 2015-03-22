using System;
using System.Globalization;

namespace Rabbit.Kernel.Utility
{
    internal sealed class Hash
    {
        #region Field

        private long _hash;

        #endregion Field

        #region Property

        public string Value
        {
            get
            {
                return _hash.ToString("x", CultureInfo.InvariantCulture);
            }
        }

        #endregion Property

        #region Overrides of Object

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            return Value;
        }

        #endregion Overrides of Object

        #region Public Method

        public void AddString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            _hash += GetStringHashCode(value);
        }

        public void AddStringInvariant(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            AddString(value.ToLowerInvariant());
        }

        public void AddTypeReference(Type type)
        {
            AddString(type.AssemblyQualifiedName);
            AddString(type.FullName);
        }

        public void AddDateTime(DateTime dateTime)
        {
            _hash += dateTime.ToBinary();
        }

        #endregion Public Method

        #region Private Method

        private static long GetStringHashCode(string s)
        {
            unchecked
            {
                var result = 352654597L;
                foreach (var ch in s)
                {
                    long h = ch.GetHashCode();
                    result = result + (h << 27) + h;
                }
                return result;
            }
        }

        #endregion Private Method
    }
}