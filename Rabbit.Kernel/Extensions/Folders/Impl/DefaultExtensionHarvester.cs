using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.Application;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rabbit.Kernel.Extensions.Folders.Impl
{
    internal sealed class DefaultExtensionHarvester : IExtensionHarvester
    {
        #region Field

        private readonly ICacheManager _cacheManager;
        private readonly IApplicationFolder _applicationFolder;

        #endregion Field

        #region Constructor

        public DefaultExtensionHarvester(ICacheManager cacheManager, IApplicationFolder applicationFolder)
        {
            _cacheManager = cacheManager;
            _applicationFolder = applicationFolder;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        #endregion Constructor

        #region Property

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public bool DisableMonitoring { get; set; }

        #endregion Property

        #region Implementation of IExtensionHarvester

        /// <summary>
        /// 收集扩展。
        /// </summary>
        /// <param name="paths">需要进行收集的路径。</param>
        /// <param name="extensionType">扩展类型。</param>
        /// <param name="manifestName">清单文件名称。</param>
        /// <param name="manifestIsOptional">清单文件是否是可选的。</param>
        /// <returns>扩展描述符集合。</returns>
        public IEnumerable<ExtensionDescriptorEntry> HarvestExtensions(IEnumerable<string> paths, string extensionType, string manifestName, bool manifestIsOptional)
        {
            return paths
                .SelectMany(path => HarvestExtensions(path, extensionType, manifestName, manifestIsOptional))
                .ToArray();
        }

        #endregion Implementation of IExtensionHarvester

        #region Private Method

        private IEnumerable<ExtensionDescriptorEntry> HarvestExtensions(string path, string extensionType, string manifestName, bool manifestIsOptional)
        {
            var key = string.Format("{0}-{1}-{2}", path, manifestName, extensionType);

            return _cacheManager.Get(key, ctx =>
            {
                if (!DisableMonitoring)
                {
                    Logger.Debug("监控虚拟路径 \"{0}\"", path);
                    ctx.Monitor(_applicationFolder.WhenPathChanges(path));
                }

                return AvailableExtensionsInFolder(path, extensionType, manifestName, manifestIsOptional).ToArray();
            });
        }

        private List<ExtensionDescriptorEntry> AvailableExtensionsInFolder(string path, string extensionType, string manifestName, bool manifestIsOptional)
        {
            Logger.Information("开始寻找扩展在 '{0}'...", path);
            var subfolderPaths = _applicationFolder.ListDirectories(path);
            var localList = new List<ExtensionDescriptorEntry>();
            foreach (var subfolderPath in subfolderPaths)
            {
                var extensionId = Path.GetFileName(subfolderPath.TrimEnd('/', '\\'));
                var manifestPath = Path.Combine(subfolderPath, manifestName);
                try
                {
                    var entry = GetExtensionDescriptor(path, extensionId, extensionType, manifestPath, manifestIsOptional);
                    if (entry == null)
                        continue;

                    var descriptor = entry.Descriptor;
                    if (descriptor == null)
                        continue;

                    if (descriptor.Path != null && !descriptor.Path.IsValidUrlSegment())
                    {
                        Logger.Error(T("模块 '{0}' 不能被加载，因为它有一个无效的路径（{1}）。它被忽略。如果指定的路径必须是一个有效的URL分类。最好的办法是坚持使用字母和数字，不要加空格。"), extensionId, descriptor.Path);
                        continue;
                    }

                    if (descriptor.Path == null)
                    {
                        descriptor.Path = descriptor.Name.IsValidUrlSegment()
                                              ? descriptor.Name
                                              : entry.Id;
                    }

                    localList.Add(entry);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "模块 '{0}' 不能被加载。它被忽略。", extensionId);
#if DEBUG
                    throw;
#endif
                }
            }
            Logger.Information("完成扩展寻找在 '{0}': {1}", path, string.Join(", ", localList.Select(d => d.Id)));
            return localList;
        }

        private ExtensionDescriptorEntry GetExtensionDescriptor(string locationPath, string extensionId, string extensionType, string manifestPath, bool manifestIsOptional)
        {
            return _cacheManager.Get(manifestPath, context =>
            {
                if (!DisableMonitoring)
                {
                    Logger.Debug("监控虚拟路径 \"{0}\"", manifestPath);
                    context.Monitor(_applicationFolder.WhenPathChanges(manifestPath));
                }

                var manifestText = _applicationFolder.ReadFile(manifestPath);
                if (manifestText == null)
                {
                    if (manifestIsOptional)
                    {
                        manifestText = string.Format("Id: {0}", extensionId);
                    }
                    else
                    {
                        return null;
                    }
                }

                var entry = new ExtensionDescriptorEntry(new ExtensionDescriptor(), extensionId, extensionType, locationPath);
                ExtensionDescriptorSerializer.Parse(manifestText, entry, Logger, T);
                return entry;
            });
        }

        #endregion Private Method
    }
}