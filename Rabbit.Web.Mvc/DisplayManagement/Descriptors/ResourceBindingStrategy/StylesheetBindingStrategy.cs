using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Web.Mvc.Utility.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors.ResourceBindingStrategy
{
    internal abstract class StaticFileBindingStrategy
    {
        private readonly IExtensionManager _extensionManager;
        private readonly ShellDescriptor _shellDescriptor;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private static readonly char[] UnsafeCharList = "/:?#[]@!&'()*+,;=\r\n\t\f\" <>.-_".ToCharArray();

        protected StaticFileBindingStrategy(IExtensionManager extensionManager, ShellDescriptor shellDescriptor, IVirtualPathProvider virtualPathProvider)
        {
            _extensionManager = extensionManager;
            _shellDescriptor = shellDescriptor;
            _virtualPathProvider = virtualPathProvider;
        }

        public abstract string GetFileExtension();

        public abstract string GetFolder();

        public abstract string GetShapePrefix();

        private static string SafeName(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? string.Empty : Strip(name, UnsafeCharList).ToLowerInvariant();
        }

        public static string GetAlternateShapeNameFromFileName(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            string shapeName;
            if (Uri.IsWellFormedUriString(fileName, UriKind.Absolute))
            {
                var uri = new Uri(fileName);
                shapeName = uri.Authority + "$" + uri.AbsolutePath + "$" + uri.Query;
            }
            else
            {
                shapeName = Path.GetFileNameWithoutExtension(fileName);
            }
            return SafeName(shapeName);
        }

        private static IEnumerable<ExtensionDescriptorEntry> Once(IEnumerable<FeatureDescriptor> featureDescriptors)
        {
            var once = new ConcurrentDictionary<string, object>();
            return featureDescriptors.Select(fd => fd.Extension).Where(ed => once.TryAdd(ed.Id, null)).ToList();
        }

        public void Discover(ShapeTableBuilder builder)
        {
            var availableFeatures = _extensionManager.AvailableFeatures();
            var activeFeatures = availableFeatures.Where(FeatureIsEnabled);
            var activeExtensions = Once(activeFeatures);

            var hits = activeExtensions.SelectMany(extensionDescriptor =>
            {
                var basePath = Path.Combine(extensionDescriptor.Location, extensionDescriptor.Id).Replace(Path.DirectorySeparatorChar, '/');
                var virtualPath = Path.Combine(basePath, GetFolder()).Replace(Path.DirectorySeparatorChar, '/');
                var shapes = _virtualPathProvider.ListFiles(virtualPath)
                    .Select(Path.GetFileName)
                    .Where(fileName => string.Equals(Path.GetExtension(fileName), GetFileExtension(), StringComparison.OrdinalIgnoreCase))
                    .Select(cssFileName => new
                    {
                        fileName = Path.GetFileNameWithoutExtension(cssFileName),
                        fileVirtualPath = Path.Combine(virtualPath, cssFileName).Replace(Path.DirectorySeparatorChar, '/'),
                        shapeType = GetShapePrefix() + GetAlternateShapeNameFromFileName(cssFileName),
                        extensionDescriptor
                    });
                return shapes;
            });

            foreach (var iter in hits)
            {
                var hit = iter;
                var featureDescriptors = hit.extensionDescriptor.Descriptor.Features.Where(fd => fd.Id == hit.extensionDescriptor.Id);
                foreach (var featureDescriptor in featureDescriptors)
                {
                    builder.Describe(iter.shapeType)
                        .From(new Feature { Descriptor = featureDescriptor })
                        .BoundAs(
                            hit.fileVirtualPath,
                            shapeDescriptor => displayContext =>
                            {
                                /*var shape = ((dynamic) displayContext.Value);
                                var output = displayContext.ViewContext.Writer;
                                ResourceDefinition resource = shape.Resource;
                                string condition = shape.Condition;
                                Dictionary<string, string> attributes = shape.TagAttributes;
                                ResourceManager.WriteResource(output, resource, hit.fileVirtualPath, condition, attributes);*/
                                return null;
                            });
                }
            }
        }

        private bool FeatureIsEnabled(FeatureDescriptor fd)
        {
            return (DefaultExtensionTypes.IsTheme(fd.Extension.ExtensionType) && (fd.Id == "TheAdmin" || fd.Id == "SafeMode")) ||
                _shellDescriptor.Features.Any(sf => sf.Name == fd.Id);
        }

        private static string Strip(string subject, params char[] stripped)
        {
            if (stripped == null || stripped.Length == 0 || string.IsNullOrEmpty(subject))
            {
                return subject;
            }

            var result = new char[subject.Length];

            var cursor = 0;
            foreach (var current in subject.Where(current => Array.IndexOf(stripped, current) < 0))
            {
                result[cursor++] = current;
            }

            return new string(result, 0, cursor);
        }
    }

    internal class StylesheetBindingStrategy : StaticFileBindingStrategy, IShapeTableProvider
    {
        public StylesheetBindingStrategy(IExtensionManager extensionManager, ShellDescriptor shellDescriptor, IVirtualPathProvider virtualPathProvider)
            : base(extensionManager, shellDescriptor, virtualPathProvider)
        {
        }

        public override string GetFileExtension()
        {
            return ".css";
        }

        public override string GetFolder()
        {
            return "Styles";
        }

        public override string GetShapePrefix()
        {
            return "Style__";
        }
    }
}