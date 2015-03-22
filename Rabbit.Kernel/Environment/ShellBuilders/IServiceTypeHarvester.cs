using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rabbit.Kernel.Environment.ShellBuilders
{
    /// <summary>
    /// 一个抽象的服务类型收集者。
    /// </summary>
    public interface IServiceTypeHarvester
    {
        /// <summary>
        /// 获取可用的服务类型。
        /// </summary>
        /// <param name="types">类型集合。</param>
        /// <returns>可用的服务类型。</returns>
        Type[] GeTypes(IEnumerable<Type> types);
    }

    /// <summary>
    /// 服务类型收集者扩展方法。
    /// </summary>
    public static class ServiceTypeHarvesterExtensions
    {
        /// <summary>
        /// 从程序集中获取可用的服务类型。
        /// </summary>
        /// <param name="serviceTypeHarvester">服务类型收集者。</param>
        /// <param name="assembly">程序集。</param>
        /// <returns>可用的服务类型。</returns>
        public static Type[] GetTypes(this IServiceTypeHarvester serviceTypeHarvester, Assembly assembly)
        {
            serviceTypeHarvester.NotNull("serviceTypeHarvester");
            assembly.NotNull("assembly");

            return serviceTypeHarvester.GeTypes(assembly.GetTypes());
        }
    }
}