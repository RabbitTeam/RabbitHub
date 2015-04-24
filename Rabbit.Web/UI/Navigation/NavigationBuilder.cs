using Rabbit.Kernel.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Web.UI.Navigation
{
    /// <summary>
    /// 导航建造者。
    /// </summary>
    public class NavigationBuilder
    {
        private List<MenuItem> Contained { get; set; }

        private readonly IList<string> _imageSets = new List<string>();

        /// <summary>
        /// 初始化一个新的导航建造者。
        /// </summary>
        public NavigationBuilder()
        {
            Contained = new List<MenuItem>();
        }

        /// <summary>
        /// 添加一个导航。
        /// </summary>
        /// <param name="caption">标题。</param>
        /// <param name="itemBuilder">导航项建造者。</param>
        /// <param name="classes">样式名称。</param>
        /// <returns>导航建造者。</returns>
        public NavigationBuilder Add(LocalizedString caption, Action<NavigationItemBuilder> itemBuilder,
            IEnumerable<string> classes = null)
        {
            return Add(caption, null, itemBuilder, classes);
        }

        /// <summary>
        /// 添加一个导航。
        /// </summary>
        /// <param name="itemBuilder">导航项建造者。</param>
        /// <param name="classes">样式名称。</param>
        /// <returns>导航建造者。</returns>
        public NavigationBuilder Add(Action<NavigationItemBuilder> itemBuilder, IEnumerable<string> classes = null)
        {
            return Add(null, itemBuilder, classes);
        }

        /// <summary>
        /// 添加一个导航。
        /// </summary>
        /// <param name="caption">标题。</param>
        /// <param name="classes">样式名称。</param>
        /// <returns>导航建造者。</returns>
        public NavigationBuilder Add(LocalizedString caption, IEnumerable<string> classes = null)
        {
            return Add(caption, x => { }, classes);
        }

        /// <summary>
        /// 添加一个导航。
        /// </summary>
        /// <param name="caption">标题。</param>
        /// <param name="position">导航位置。</param>
        /// <param name="itemBuilder">导航项建造者。</param>
        /// <param name="classes">样式名称。</param>
        /// <returns>导航建造者。</returns>
        public NavigationBuilder Add(LocalizedString caption, string position, Action<NavigationItemBuilder> itemBuilder, IEnumerable<string> classes = null)
        {
            var childBuilder = new NavigationItemBuilder();

            childBuilder.Caption(caption);
            childBuilder.Position(position);
            itemBuilder(childBuilder);
            Contained.AddRange(childBuilder.Build());

            if (classes == null)
                return this;
            foreach (var className in classes)
                childBuilder.AddClass(className);

            return this;
        }

        /// <summary>
        /// 删除一个导航。
        /// </summary>
        /// <param name="item">导航项。</param>
        /// <returns>导航建造者。</returns>
        public NavigationBuilder Remove(MenuItem item)
        {
            Contained.Remove(item);
            return this;
        }

        /// <summary>
        /// 添加图片集。
        /// </summary>
        /// <param name="imageSet">图片集。</param>
        /// <returns>导航建造者。</returns>

        public NavigationBuilder AddImageSet(string imageSet)
        {
            _imageSets.Add(imageSet);
            return this;
        }

        /// <summary>
        /// 生成导航。
        /// </summary>
        /// <returns>导航集合。</returns>
        public IEnumerable<MenuItem> Build()
        {
            return (Contained ?? Enumerable.Empty<MenuItem>()).ToList();
        }

        /// <summary>
        /// 生成图片集。
        /// </summary>
        /// <returns>图片集。</returns>
        public IEnumerable<string> BuildImageSets()
        {
            return _imageSets.Distinct();
        }
    }
}