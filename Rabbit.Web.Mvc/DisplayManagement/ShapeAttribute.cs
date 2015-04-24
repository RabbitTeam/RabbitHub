using System;

namespace Rabbit.Web.Mvc.DisplayManagement
{
    /// <summary>
    /// 形状标记。
    /// </summary>
    public sealed class ShapeAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个新的形状标记。
        /// </summary>
        public ShapeAttribute()
        {
        }

        /// <summary>
        /// 初始化一个新的形状标记。
        /// </summary>
        /// <param name="shapeType">形状类型。</param>
        public ShapeAttribute(string shapeType)
        {
            ShapeType = shapeType;
        }

        /// <summary>
        /// 形状类型。
        /// </summary>
        public string ShapeType { get; private set; }
    }
}