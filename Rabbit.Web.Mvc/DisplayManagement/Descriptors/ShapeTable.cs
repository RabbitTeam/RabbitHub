using System.Collections.Generic;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors
{
    /// <summary>
    /// 形状表格。
    /// </summary>
    public sealed class ShapeTable
    {
        /// <summary>
        /// 描述符字典表。
        /// </summary>
        public IDictionary<string, ShapeDescriptor> Descriptors { get; set; }

        /// <summary>
        /// 绑定字典表。
        /// </summary>
        public IDictionary<string, ShapeBinding> Bindings { get; set; }
    }
}