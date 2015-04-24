using Rabbit.Kernel;
using System.Collections.Generic;

namespace Rabbit.Web.Mvc.Mvc.ModelBinders
{
    /// <summary>
    /// 一个抽象的模型绑定提供程序。
    /// </summary>
    public interface IModelBinderProvider : IDependency
    {
        /// <summary>
        /// 获取模型绑定程序。
        /// </summary>
        /// <returns>绑定程序集合。</returns>
        IEnumerable<ModelBinderDescriptor> GetModelBinders();
    }
}