using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors
{
    /// <summary>
    /// 放置信息。
    /// </summary>
    public sealed class PlacementInfo
    {
        private static readonly char[] Delimiters = { ':', '#', '@' };

        /// <summary>
        /// 初始化一个新的放置信息。
        /// </summary>
        public PlacementInfo()
        {
            Alternates = Enumerable.Empty<string>();
            Wrappers = Enumerable.Empty<string>();
        }

        /// <summary>
        /// 位置。
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 源。
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 形状类型，
        /// </summary>
        public string ShapeType { get; set; }

        /// <summary>
        /// 候补形状。
        /// </summary>
        public IEnumerable<string> Alternates { get; set; }

        /// <summary>
        /// 包装。
        /// </summary>
        public IEnumerable<string> Wrappers { get; set; }

        /// <summary>
        /// 获取区域。
        /// </summary>
        /// <returns>区域。</returns>
        public string GetZone()
        {
            var firstDelimiter = Location.IndexOfAny(Delimiters);
            return firstDelimiter == -1 ? Location.TrimStart('/') : Location.Substring(0, firstDelimiter).TrimStart('/');
        }

        /// <summary>
        /// 获取位置。
        /// </summary>
        /// <returns>位置。</returns>
        public string GetPosition()
        {
            var contentDelimiter = Location.IndexOf(':');
            if (contentDelimiter == -1)
            {
                return string.Empty;
            }

            var secondDelimiter = Location.IndexOfAny(Delimiters, contentDelimiter + 1);
            return secondDelimiter == -1 ? Location.Substring(contentDelimiter + 1) : Location.Substring(contentDelimiter + 1, secondDelimiter - contentDelimiter - 1);
        }

        /// <summary>
        /// 是否是布局区域。
        /// </summary>
        /// <returns>如果是返回true，否则返回false。</returns>
        public bool IsLayoutZone()
        {
            return Location.StartsWith("/");
        }

        /// <summary>
        /// 获取标签。
        /// </summary>
        /// <returns>标签。</returns>
        public string GetTab()
        {
            var tabDelimiter = Location.IndexOf('#');
            if (tabDelimiter == -1)
            {
                return string.Empty;
            }

            var nextDelimiter = Location.IndexOfAny(Delimiters, tabDelimiter + 1);
            return nextDelimiter == -1 ? Location.Substring(tabDelimiter + 1) : Location.Substring(tabDelimiter + 1, nextDelimiter - tabDelimiter - 1);
        }

        /// <summary>
        /// 获取组。
        /// </summary>
        /// <returns>组。</returns>
        public string GetGroup()
        {
            var groupDelimiter = Location.IndexOf('@');
            if (groupDelimiter == -1)
            {
                return string.Empty;
            }

            var nextDelimiter = Location.IndexOfAny(Delimiters, groupDelimiter + 1);
            return nextDelimiter == -1 ? Location.Substring(groupDelimiter + 1) : Location.Substring(groupDelimiter + 1, nextDelimiter - groupDelimiter - 1);
        }
    }
}