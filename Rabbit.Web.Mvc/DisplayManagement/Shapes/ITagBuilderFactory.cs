using Rabbit.Kernel;
using System.Web;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.DisplayManagement.Shapes
{
    /// <summary>
    /// 一个抽象的标签建造器工厂。
    /// </summary>
    public interface ITagBuilderFactory : IDependency
    {
        /// <summary>
        /// 创建标签建造器。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <param name="tagName">标签名称。</param>
        /// <returns>标签建造器。</returns>
        RabbitTagBuilder Create(dynamic shape, string tagName);
    }

    /// <summary>
    /// 标签建造器。
    /// </summary>
    public sealed class RabbitTagBuilder : TagBuilder
    {
        /// <summary>
        /// 初始化一个新的标签建造器。
        /// </summary>
        /// <param name="tagName">标签名称。</param>
        public RabbitTagBuilder(string tagName)
            : base(tagName)
        {
        }

        /// <summary>
        /// 起始元素。
        /// </summary>
        public IHtmlString StartElement { get { return new HtmlString(ToString(TagRenderMode.StartTag)); } }

        /// <summary>
        /// 结束元素。
        /// </summary>
        public IHtmlString EndElement { get { return new HtmlString(ToString(TagRenderMode.EndTag)); } }
    }
}