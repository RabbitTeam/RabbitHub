using System;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ModelBinders
{
    /// <summary>
    /// 模型绑定程序描述符。
    /// </summary>
    public sealed class ModelBinderDescriptor
    {
        /// <summary>
        /// 类型。
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 模型绑定者。
        /// </summary>
        public IModelBinder ModelBinder { get; set; }
    }
}