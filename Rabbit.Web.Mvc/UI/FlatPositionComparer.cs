using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Web.Mvc.UI
{
    /// <summary>
    /// 屏幕位置比较器。
    /// </summary>
    public sealed class FlatPositionComparer : IComparer<string>
    {
        #region Implementation of IComparer<in string>

        /// <summary>
        /// 比较两个对象并返回一个值，指示一个对象是小于、等于还是大于另一个对象。
        /// </summary>
        /// <returns>
        /// 一个带符号整数，它指示 <paramref name="x"/> 与 <paramref name="y"/> 的相对值，如下表所示。值含义小于零<paramref name="x"/> 小于 <paramref name="y"/>。零<paramref name="x"/> 等于 <paramref name="y"/>。大于零<paramref name="x"/> 大于 <paramref name="y"/>。
        /// </returns>
        /// <param name="x">要比较的第一个对象。</param><param name="y">要比较的第二个对象。</param>
        public int Compare(string x, string y)
        {
            if (x == y)
                return 0;

            // null == "before; "" == "0"
            x = x == null
                ? "before"
                : x.Trim().Length == 0 ? "0" : x.Trim(':').TrimEnd('.');
            y = y == null
                ? "before"
                : y.Trim().Length == 0 ? "0" : y.Trim(':').TrimEnd('.');

            var xParts = x.Split(new[] { '.', ':' });
            var yParts = y.Split(new[] { '.', ':' });
            for (var i = 0; i < xParts.Count(); i++)
            {
                if (yParts.Length < i + 1)
                    return 1;

                int xPos;
                int yPos;
                var xPart = string.IsNullOrWhiteSpace(xParts[i]) ? "before" : xParts[i];
                var yPart = string.IsNullOrWhiteSpace(yParts[i]) ? "before" : yParts[i];

                xPart = NormalizeKnownPartitions(xPart);
                yPart = NormalizeKnownPartitions(yPart);

                var xIsInt = int.TryParse(xPart, out xPos);
                var yIsInt = int.TryParse(yPart, out yPos);

                if (!xIsInt && !yIsInt)
                    return string.Compare(string.Join(".", xParts), string.Join(".", yParts), StringComparison.OrdinalIgnoreCase);
                if (!xIsInt || (yIsInt && xPos > yPos))
                    return 1;
                if (!yIsInt || xPos < yPos)
                    return -1;
            }

            if (xParts.Length < yParts.Length)
                return -1;

            return 0;
        }

        #endregion Implementation of IComparer<in string>

        #region Private Method

        private static string NormalizeKnownPartitions(string partition)
        {
            if (partition.Length < 5) //已知的分区很长
                return partition;

            if (string.Equals(partition, "before", StringComparison.OrdinalIgnoreCase))
                return "-9999";
            if (string.Equals(partition, "after", StringComparison.OrdinalIgnoreCase))
                return "9999";

            return partition;
        }

        #endregion Private Method
    }
}