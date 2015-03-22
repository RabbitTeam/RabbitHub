using Rabbit.Kernel.Caching;
using Rabbit.Kernel.FileSystems.AppData;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Rabbit.Kernel.FileSystems.Dependencies.Impl
{
    internal sealed class DefaultExtensionDependenciesManager : IExtensionDependenciesManager
    {
        #region Field

        private const string BasePath = "Dependencies";
        private const string FileName = "dependencies.compiled.xml";
        private readonly ICacheManager _cacheManager;
        private readonly IAppDataFolder _appDataFolder;
        private readonly InvalidationToken _writeThroughToken;

        private readonly string _persistencePath;

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        public bool DisableMonitoring { get; set; }

        #endregion Property

        #region Constructor

        public DefaultExtensionDependenciesManager(ICacheManager cacheManager, IAppDataFolder appDataFolder)
        {
            _cacheManager = cacheManager;
            _appDataFolder = appDataFolder;
            _writeThroughToken = new InvalidationToken();

            Logger = NullLogger.Instance;

            _persistencePath = appDataFolder.Combine(BasePath, FileName);
        }

        #endregion Constructor

        #region Implementation of IExtensionDependenciesManager

        /// <summary>
        /// 存储依赖。
        /// </summary>
        /// <param name="dependencyDescriptors">依赖描述符集合。</param>
        /// <param name="fileHashProvider">文件哈希值提供程序。</param>
        public void StoreDependencies(IEnumerable<DependencyDescriptor> dependencyDescriptors, Func<DependencyDescriptor, string> fileHashProvider)
        {
            Logger.Information("存储模块依赖文件。");

            var newDocument = CreateDocument(dependencyDescriptors, fileHashProvider);
            var previousDocument = ReadDocument(_persistencePath);
            if (XNode.DeepEquals(newDocument.Root, previousDocument.Root))
            {
                Logger.Debug("现有的文件与新的是相同的。跳过保存。");
            }
            else
            {
                WriteDocument(_persistencePath, newDocument);
            }

            Logger.Information("完成模块依赖文件的存储。");
        }

        /// <summary>
        /// 获取扩展的虚拟路径依赖项。
        /// </summary>
        /// <param name="extensionId">扩展Id。</param>
        /// <returns>虚拟路径集合。</returns>
        public IEnumerable<string> GetVirtualPathDependencies(string extensionId)
        {
            var descriptor = GetDescriptor(extensionId);
            if (descriptor != null && IsSupportedLoader(descriptor.LoaderName))
            {
                //目前，我们返回每一个模块的同一个文件。一种改进是将返回每个模块一个特定的文件（这将减少，当模块在磁盘上的改变需要重新编译的次数）。
                yield return _appDataFolder.GetVirtualPath(_persistencePath);
            }
        }

        /// <summary>
        /// 获取一个已经激活的扩展描述符。
        /// </summary>
        /// <param name="extensionId">扩展Id。</param>
        /// <returns>已经激活的扩展描述符。</returns>
        public ActivatedExtensionDescriptor GetDescriptor(string extensionId)
        {
            return LoadDescriptors().FirstOrDefault(d => d.ExtensionId.Equals(extensionId, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Implementation of IExtensionDependenciesManager

        #region Private Method

        private IEnumerable<ActivatedExtensionDescriptor> LoadDescriptors()
        {
            return _cacheManager.Get(_persistencePath, ctx =>
            {
                _appDataFolder.CreateDirectory(BasePath);

                if (!DisableMonitoring)
                {
                    ctx.Monitor(_appDataFolder.WhenPathChanges(ctx.Key));
                }

                _writeThroughToken.IsCurrent = true;
                ctx.Monitor(_writeThroughToken);

                return ReadDescriptors(ctx.Key).ToList();
            });
        }

        private static XDocument CreateDocument(IEnumerable<DependencyDescriptor> dependencies, Func<DependencyDescriptor, string> fileHashProvider)
        {
            Func<string, XName> ns = (XName.Get);

            var elements = dependencies
                .Where(dep => IsSupportedLoader(dep.LoaderName))
                .OrderBy(dep => dep.Name, StringComparer.OrdinalIgnoreCase)
                .Select(descriptor =>
                        new XElement(ns("Dependency"),
                            new XElement(ns("ExtensionId"), descriptor.Name),
                            new XElement(ns("LoaderName"), descriptor.LoaderName),
                            new XElement(ns("VirtualPath"), descriptor.VirtualPath),
                            new XElement(ns("Hash"), fileHashProvider(descriptor))));

            return new XDocument(new XElement(ns("Dependencies"), elements.ToArray()));
        }

        private IEnumerable<ActivatedExtensionDescriptor> ReadDescriptors(string persistancePath)
        {
            Func<string, XName> ns = (XName.Get);
            Func<XElement, string, string> elem = (e, name) => e.Element(ns(name)).Value;

            var document = ReadDocument(persistancePath);
            return document
                .Elements(ns("Dependencies"))
                .Elements(ns("Dependency"))
                .Select(e => new ActivatedExtensionDescriptor
                {
                    ExtensionId = elem(e, "ExtensionId"),
                    VirtualPath = elem(e, "VirtualPath"),
                    LoaderName = elem(e, "LoaderName"),
                    Hash = elem(e, "Hash"),
                }).ToList();
        }

        private static bool IsSupportedLoader(string loaderName)
        {
            return
                loaderName == "DynamicExtensionLoader" ||
                loaderName == "PrecompiledExtensionLoader";
        }

        private void WriteDocument(string persistancePath, XDocument document)
        {
            _writeThroughToken.IsCurrent = false;
            _appDataFolder.CreateFile(persistancePath, stream => document.Save(stream, SaveOptions.None));
        }

        private XDocument ReadDocument(string persistancePath)
        {
            if (!_appDataFolder.FileExists(persistancePath))
                return new XDocument();

            try
            {
                return _appDataFolder.OpenFileFunc(persistancePath, XDocument.Load);
            }
            catch (Exception e)
            {
                Logger.Information(e, "读取文件 \"{0}\" 时发生了错误。", persistancePath);
                return new XDocument();
            }
        }

        #endregion Private Method

        #region Help Class

        private sealed class InvalidationToken : IVolatileToken
        {
            public bool IsCurrent { get; set; }
        }

        #endregion Help Class
    }
}