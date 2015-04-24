using Rabbit.Kernel;
using System.Collections.Generic;

namespace Rabbit.Web.Mvc.Mvc.ModelBinders
{
    /// <summary>
    /// 一个抽象的模型绑定程序发布者。
    /// </summary>
    public interface IModelBinderPublisher : IDependency
    {
        /// <summary>
        /// 发布。
        /// </summary>
        /// <param name="binders">绑定信息。</param>
        void Publish(IEnumerable<ModelBinderDescriptor> binders);
    }
}