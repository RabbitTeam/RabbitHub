using Rabbit.Kernel.Extensions.Models;
using System;
using System.Collections.Generic;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors
{
    /// <summary>
    /// 形状候补。
    /// </summary>
    public class ShapeAlteration
    {
        private readonly IList<Action<ShapeDescriptor>> _configurations;

        /// <summary>
        /// 初始化一个新的形状候补。
        /// </summary>
        /// <param name="shapeType">形状类型。</param>
        /// <param name="feature">特性。</param>
        /// <param name="configurations">配置委托。</param>
        public ShapeAlteration(string shapeType, Feature feature, IList<Action<ShapeDescriptor>> configurations)
        {
            _configurations = configurations;
            ShapeType = shapeType;
            Feature = feature;
        }

        /// <summary>
        /// 形状类型。
        /// </summary>
        public string ShapeType { get; private set; }

        /// <summary>
        /// 特性。
        /// </summary>
        public Feature Feature { get; private set; }

        /// <summary>
        /// 应用。
        /// </summary>
        /// <param name="descriptor">形状描述符。</param>
        public void Alter(ShapeDescriptor descriptor)
        {
            foreach (var configuration in _configurations)
            {
                configuration(descriptor);
            }
        }
    }
}