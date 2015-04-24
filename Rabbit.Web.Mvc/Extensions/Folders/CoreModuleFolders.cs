using Rabbit.Kernel.Extensions.Folders;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Web.Mvc.Utility.Extensions;
using System.Collections.Generic;

namespace Rabbit.Web.Mvc.Extensions.Folders
{
    internal sealed class CoreModuleFolders : IExtensionFolders
    {
        #region Field

        private readonly IEnumerable<string> _paths;
        private readonly IExtensionHarvester _extensionHarvester;

        #endregion Field

        #region Constructor

        public CoreModuleFolders(IEnumerable<string> paths, IExtensionHarvester extensionHarvester)
        {
            _paths = paths;
            _extensionHarvester = extensionHarvester;
        }

        #endregion Constructor

        #region Implementation of IExtensionFolders

        /// <summary>
        /// 可用的扩展。
        /// </summary>
        /// <returns>扩展描述条目符集合。</returns>
        public IEnumerable<ExtensionDescriptorEntry> AvailableExtensions()
        {
            return _extensionHarvester.HarvestExtensions(_paths, DefaultExtensionTypes.Module, "Module.txt", false);
        }

        #endregion Implementation of IExtensionFolders
    }
}