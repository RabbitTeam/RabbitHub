using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Environment.ShellBuilders;
using System.Collections.Generic;

namespace Rabbit.Components.Logging.NLog
{
    internal sealed class MinimumShellDescriptorProvider : IMinimumShellDescriptorProvider
    {
        #region Implementation of IMinimumShellDescriptorProvider

        /// <summary>
        /// 获取外壳描述符。
        /// </summary>
        /// <param name="features">外壳特性描述符。</param>
        public void GetFeatures(ICollection<ShellFeature> features)
        {
            features.Add(new ShellFeature { Name = "Rabbit.Components.Logging.NLog" });
        }

        #endregion Implementation of IMinimumShellDescriptorProvider
    }
}