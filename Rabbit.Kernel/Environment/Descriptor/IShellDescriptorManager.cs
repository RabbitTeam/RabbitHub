using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Events;
using System.Collections.Generic;

namespace Rabbit.Kernel.Environment.Descriptor
{
    /// <summary>
    /// 一个抽象的外壳描述符管理者。
    /// </summary>
    public interface IShellDescriptorManager : IDependency
    {
        /// <summary>
        /// 获取当前外壳描述符。
        /// </summary>
        /// <returns>外壳描述符。</returns>
        ShellDescriptor GetShellDescriptor();

        /// <summary>
        /// 更新外壳描述符。
        /// </summary>
        /// <param name="serialNumber">序列号。</param>
        /// <param name="enabledFeatures">需要开启的特性。</param>
        void UpdateShellDescriptor(int serialNumber, IEnumerable<ShellFeature> enabledFeatures);
    }

    /// <summary>
    /// 一个抽象的外壳描述符管理者事件处理程序。
    /// </summary>
    public interface IShellDescriptorManagerEventHandler : IEventHandler
    {
        /// <summary>
        /// 当外壳描述符发生变更时执行。
        /// </summary>
        /// <param name="descriptor">新的外壳描述符。</param>
        /// <param name="tenant">租户名称。</param>
        void Changed(ShellDescriptor descriptor, string tenant);
    }
}