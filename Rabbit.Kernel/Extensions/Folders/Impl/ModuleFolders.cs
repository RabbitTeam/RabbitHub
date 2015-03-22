using Rabbit.Kernel.Extensions.Models;
using System.Collections.Generic;

namespace Rabbit.Kernel.Extensions.Folders.Impl
{
    internal sealed class ModuleFolders : IExtensionFolders
    {
        #region Field

        private readonly IEnumerable<string> _paths;
        private readonly IExtensionHarvester _extensionHarvester;

        #endregion Field

        #region Constructor

        public ModuleFolders(IEnumerable<string> paths, IExtensionHarvester extensionHarvester)
        {
            _paths = paths;
            _extensionHarvester = extensionHarvester;
        }

        #endregion Constructor

        #region Implementation of IExtensionFolders

        /// <summary>
        /// 可用的扩展。
        /// </summary>
        /// <returns>扩展描述符条目集合。</returns>
        public IEnumerable<ExtensionDescriptorEntry> AvailableExtensions()
        {
            return _extensionHarvester.HarvestExtensions(_paths, "Module", "Module.txt", false);
        }

        #endregion Implementation of IExtensionFolders
    }
}