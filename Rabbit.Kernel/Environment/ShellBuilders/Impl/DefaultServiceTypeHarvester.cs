using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Environment.ShellBuilders.Impl
{
    internal sealed class DefaultServiceTypeHarvester : IServiceTypeHarvester
    {
        #region Implementation of IServiceTypeHarvester

        /// <summary>
        /// 获取可用的服务类型。
        /// </summary>
        /// <param name="types">类型集合。</param>
        /// <returns>可用的服务类型。</returns>
        public Type[] GeTypes(IEnumerable<Type> types)
        {
            types = types.NotNull("types").ToArray();
            return types.Where(i => i.IsClass && !i.IsAbstract).ToArray();
        }

        #endregion Implementation of IServiceTypeHarvester
    }
}