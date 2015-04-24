using Rabbit.Kernel.Extensions.Models;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors
{
    /// <summary>
    /// 形状表格建造器。
    /// </summary>
    public sealed class ShapeTableBuilder
    {
        private readonly IList<ShapeAlterationBuilder> _alterationBuilders = new List<ShapeAlterationBuilder>();
        private readonly Feature _feature;

        /// <summary>
        /// 初始化一个新的形状表格建造器。
        /// </summary>
        /// <param name="feature">特性。</param>
        public ShapeTableBuilder(Feature feature)
        {
            _feature = feature;
        }

        /// <summary>
        /// 发现。
        /// </summary>
        /// <param name="shapeType">形状类型。</param>
        /// <returns>形状候补建造器。</returns>
        public ShapeAlterationBuilder Describe(string shapeType)
        {
            var alterationBuilder = new ShapeAlterationBuilder(_feature, shapeType);
            _alterationBuilders.Add(alterationBuilder);
            return alterationBuilder;
        }

        /// <summary>
        /// 生成候补形状。
        /// </summary>
        /// <returns>形状候补集合。</returns>
        public IEnumerable<ShapeAlteration> BuildAlterations()
        {
            return _alterationBuilders.Select(b => b.Build());
        }
    }
}