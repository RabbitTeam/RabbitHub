using System.Collections.Generic;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ModelBinders.Impl
{
    internal sealed class DefaultModelBinderPublisher : IModelBinderPublisher
    {
        private readonly ModelBinderDictionary _binders;

        public DefaultModelBinderPublisher(ModelBinderDictionary binders)
        {
            _binders = binders;
        }

        #region Implementation of IModelBinderPublisher

        /// <summary>
        /// 发布。
        /// </summary>
        /// <param name="binders">绑定信息。</param>
        public void Publish(IEnumerable<ModelBinderDescriptor> binders)
        {
            foreach (var descriptor in binders)
            {
                _binders[descriptor.Type] = descriptor.ModelBinder;
            }
        }

        #endregion Implementation of IModelBinderPublisher
    }
}