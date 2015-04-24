using System.Linq;

namespace Rabbit.Web.Mvc.DisplayManagement.Shapes.Impl
{
    internal sealed class DefaultTagBuilderFactory : ITagBuilderFactory
    {
        #region Implementation of ITagBuilderFactory

        /// <summary>
        /// 创建标签建造器。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <param name="tagName">标签名称。</param>
        /// <returns>标签建造器。</returns>
        public RabbitTagBuilder Create(dynamic shape, string tagName)
        {
            var tagBuilder = new RabbitTagBuilder(tagName);
            tagBuilder.MergeAttributes(shape.Attributes, false);
            foreach (var cssClass in shape.Classes ?? Enumerable.Empty<string>())
                tagBuilder.AddCssClass(cssClass);
            if (!string.IsNullOrEmpty(shape.Id))
                tagBuilder.GenerateId(shape.Id);
            return tagBuilder;
        }

        #endregion Implementation of ITagBuilderFactory
    }
}