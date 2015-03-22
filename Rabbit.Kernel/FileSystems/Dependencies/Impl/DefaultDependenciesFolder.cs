using Rabbit.Kernel.Caching;
using Rabbit.Kernel.FileSystems.AppData;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Rabbit.Kernel.FileSystems.Dependencies.Impl
{
    internal sealed class DefaultDependenciesFolder : IDependenciesFolder
    {
        #region Field

        private const string BasePath = "Dependencies";
        private const string FileName = "dependencies.xml";
        private readonly ICacheManager _cacheManager;
        private readonly IAppDataFolder _appDataFolder;
        private readonly InvalidationToken _writeThroughToken;

        private readonly string _persistencePath;

        #endregion Field

        #region Constructor

        public DefaultDependenciesFolder(ICacheManager cacheManager, IAppDataFolder appDataFolder)
        {
            _cacheManager = cacheManager;
            _appDataFolder = appDataFolder;
            _writeThroughToken = new InvalidationToken();

            T = NullLocalizer.Instance;

            _persistencePath = appDataFolder.Combine(BasePath, FileName);
        }

        #endregion Constructor

        #region Property

        public Localizer T { get; set; }

        public bool DisableMonitoring { get; set; }

        #endregion Property

        #region Implementation of IDependenciesFolder

        /// <summary>
        /// 获取一个依赖项描述符。
        /// </summary>
        /// <param name="moduleName">模块名称。</param>
        /// <returns>依赖项描述符。</returns>
        public DependencyDescriptor GetDescriptor(string moduleName)
        {
            return LoadDescriptors().SingleOrDefault(d => StringComparer.OrdinalIgnoreCase.Equals(d.Name, moduleName));
        }

        /// <summary>
        /// 装载所有依赖项描述符。
        /// </summary>
        /// <returns>依赖项描述符集合。</returns>
        public IEnumerable<DependencyDescriptor> LoadDescriptors()
        {
            return _cacheManager.Get(_persistencePath,
                                     ctx =>
                                     {
                                         _appDataFolder.CreateDirectory(BasePath);

                                         if (!DisableMonitoring)
                                         {
                                             ctx.Monitor(_appDataFolder.WhenPathChanges(ctx.Key));
                                         }

                                         _writeThroughToken.IsCurrent = true;
                                         ctx.Monitor(_writeThroughToken);

                                         return ReadDependencies(ctx.Key).ToArray();
                                     });
        }

        /// <summary>
        /// 存储依赖项描述符。
        /// </summary>
        /// <param name="dependencyDescriptors">依赖项描述符集合。</param>
        public void StoreDescriptors(IEnumerable<DependencyDescriptor> dependencyDescriptors)
        {
            dependencyDescriptors = dependencyDescriptors.NotNull("dependencyDescriptors").ToArray();

            var existingDescriptors = LoadDescriptors().OrderBy(d => d.Name);
            var newDescriptors = dependencyDescriptors.OrderBy(d => d.Name);

            if (!newDescriptors.SequenceEqual(existingDescriptors, new DependencyDescriptorComparer()))
            {
                WriteDependencies(_persistencePath, dependencyDescriptors);
            }
        }

        #endregion Implementation of IDependenciesFolder

        #region Private Method

        private IEnumerable<DependencyDescriptor> ReadDependencies(string persistancePath)
        {
            Func<string, XName> ns = (XName.Get);
            Func<XElement, string, string> elem = (e, name) => e.Element(ns(name)).Value;

            if (!_appDataFolder.FileExists(persistancePath))
                return Enumerable.Empty<DependencyDescriptor>();

            return _appDataFolder.OpenFileFunc(persistancePath, stream =>
            {
                var document = XDocument.Load(stream);
                return document
                    .Elements(ns("Dependencies"))
                    .Elements(ns("Dependency"))
                    .Select(e => new DependencyDescriptor
                    {
                        Name = elem(e, "ModuleName"),
                        VirtualPath = elem(e, "VirtualPath"),
                        LoaderName = elem(e, "LoaderName"),
                        References = e.Elements(ns("References")).Elements(ns("Reference")).Select(r => new DependencyReferenceDescriptor
                        {
                            Name = elem(r, "Name"),
                            LoaderName = elem(r, "LoaderName"),
                            VirtualPath = elem(r, "VirtualPath")
                        })
                    }).ToList();
            });
        }

        private void WriteDependencies(string persistancePath, IEnumerable<DependencyDescriptor> dependencies)
        {
            Func<string, XName> ns = (XName.Get);

            var document = new XDocument();
            document.Add(new XElement(ns("Dependencies")));
            var elements = dependencies.Select(d => new XElement("Dependency",
                                                                 new XElement(ns("ModuleName"), d.Name),
                                                                 new XElement(ns("VirtualPath"), d.VirtualPath),
                                                                 new XElement(ns("LoaderName"), d.LoaderName),
                                                                 new XElement(ns("References"), d.References
                                                                     .Select(r => new XElement(ns("Reference"),
                                                                        new XElement(ns("Name"), r.Name),
                                                                        new XElement(ns("LoaderName"), r.LoaderName),
                                                                        new XElement(ns("VirtualPath"), r.VirtualPath))).ToArray())));

            if (document.Root != null)
                document.Root.Add(elements);
            _appDataFolder.CreateFile(persistancePath, stream => document.Save(stream, SaveOptions.None));

            //使缓存失效
            _writeThroughToken.IsCurrent = false;
        }

        #endregion Private Method

        #region Help Class

        private sealed class InvalidationToken : IVolatileToken
        {
            public bool IsCurrent { get; set; }
        }

        private sealed class DependencyDescriptorComparer : EqualityComparer<DependencyDescriptor>
        {
            private readonly ReferenceDescriptorComparer _referenceDescriptorComparer = new ReferenceDescriptorComparer();

            public override bool Equals(DependencyDescriptor x, DependencyDescriptor y)
            {
                return
                    StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name) &&
                    StringComparer.OrdinalIgnoreCase.Equals(x.LoaderName, y.LoaderName) &&
                    StringComparer.OrdinalIgnoreCase.Equals(x.VirtualPath, y.VirtualPath) &&
                    x.References.SequenceEqual(y.References, _referenceDescriptorComparer);
            }

            public override int GetHashCode(DependencyDescriptor obj)
            {
                if (obj != null)
                    return
                        StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) ^
                        StringComparer.OrdinalIgnoreCase.GetHashCode(obj.LoaderName) ^
                        StringComparer.OrdinalIgnoreCase.GetHashCode(obj.VirtualPath) ^
                        obj.References.Aggregate(0, (a, entry) => a + _referenceDescriptorComparer.GetHashCode(entry));
                return 0;
            }
        }

        private sealed class ReferenceDescriptorComparer : EqualityComparer<DependencyReferenceDescriptor>
        {
            public override bool Equals(DependencyReferenceDescriptor x, DependencyReferenceDescriptor y)
            {
                return
                    StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name) &&
                    StringComparer.OrdinalIgnoreCase.Equals(x.LoaderName, y.LoaderName) &&
                    StringComparer.OrdinalIgnoreCase.Equals(x.VirtualPath, y.VirtualPath);
            }

            public override int GetHashCode(DependencyReferenceDescriptor obj)
            {
                if (obj != null)
                    return
                        StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) ^
                        StringComparer.OrdinalIgnoreCase.GetHashCode(obj.LoaderName) ^
                        StringComparer.OrdinalIgnoreCase.GetHashCode(obj.VirtualPath);
                return 0;
            }
        }

        #endregion Help Class
    }
}