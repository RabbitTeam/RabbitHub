using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rabbit.Kernel.Extensions.Folders.Impl
{
    internal sealed class ExtensionDescriptorSerializer
    {
        #region Field

        private const string NameSection = "name";
        private const string PathSection = "path";
        private const string DescriptionSection = "description";
        private const string VersionSection = "version";
        private const string KernelVersionSection = "kernelversion";
        private const string AuthorSection = "author";
        private const string WebsiteSection = "website";
        private const string TagsSection = "tags";
        private const string DependenciesSection = "dependencies";
        private const string FeaturesSection = "features";
        private const string RuntimeSection = "runtime";

        private const string CategorySection = "category";
        private const string PrioritySection = "priority";
        private const string FeatureNameSection = "featurename";
        private const string FeatureDescriptionSection = "featuredescription";

        private static ILogger _logger;
        private static Localizer _localizer;

        #endregion Field

        #region Public Method

        public static void Parse(string text, ExtensionDescriptorEntry entry, ILogger logger, Localizer localizer)
        {
            _logger = logger;
            _localizer = localizer;

            var descriptor = entry.Descriptor;
            var manifest = ParseManifest(text, (key, value) =>
            {
                descriptor[key] = value;
            });

            descriptor.Name = GetValue(manifest, NameSection);
            descriptor.Path = GetValue(manifest, PathSection);
            descriptor.Description = GetValue(manifest, DescriptionSection);
            descriptor.Version = GetExtensionVersion(entry.Id, manifest);
            descriptor.KernelVersion = GetKernelVersion(entry.Id, manifest);
            descriptor.Author = GetValue(manifest, AuthorSection);
            descriptor.WebSite = GetValue(manifest, WebsiteSection);
            descriptor.Tags = GetValue(manifest, TagsSection);
            //            descriptor.Runtime = GetRuntime(manifest);

            descriptor.Features = GetFeaturesForExtension(manifest, entry);
        }

        #endregion Public Method

        #region Private Method

        private static Version GetExtensionVersion(string extensionId, IDictionary<string, string> fields)
        {
            var version = GetVersion(GetValue(fields, VersionSection));
            if (version == null)
                _logger.Error(_localizer("没有为扩展 {0} 指定版本号。", extensionId));
            return version;
        }

        private static VersionRange GetKernelVersion(string extensionId, IDictionary<string, string> fields)
        {
            var versionRangeString = GetValue(fields, KernelVersionSection);
            var versions = versionRangeString.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(GetVersion).ToArray();

            var minVersion = versions.FirstOrDefault();
            var maxVersion = versions.Length > 1 ? versions.Skip(1).FirstOrDefault() : minVersion;

            if (minVersion == null && maxVersion == null)
            {
                _logger.Error(_localizer("没有为扩展 {0} 指定内核版本号。", extensionId));
            }

            return new VersionRange(minVersion, maxVersion);
        }

        private static Version GetVersion(string value)
        {
            Version version;
            Version.TryParse(value, out version);
            return version;
        }

        /*        private static RuntimeDescriptor GetRuntime(IDictionary<string, string> fields)
                {
                    var runtimString = GetValue(fields, RuntimeSection);
                    AssemblyDescriptor[] assemblyDescriptors = null;
                    if (!string.IsNullOrWhiteSpace(runtimString))
                    {
                        var assemblys = runtimString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                        assemblyDescriptors = assemblys.Select(i =>
                        {
                            try
                            {
                                return new AssemblyDescriptor(i);
                            }
                            catch
                            {
                                return null;
                            }
                        }).ToArray();
                    }

                    if (assemblyDescriptors == null || !assemblyDescriptors.Any())
                    {
                        todo当找不到任何运行时信息时应该进行日志记录或异常抛出。
                    }

                    return new RuntimeDescriptor
                    {
                        Assemblies = assemblyDescriptors
                    };
                }*/

        private static Dictionary<string, string> ParseManifest(string manifestText, Action<string, string> defaultAction)
        {
            var manifest = new Dictionary<string, string>();

            using (var reader = new StringReader(manifestText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var field = line.Split(new[] { ":" }, 2, StringSplitOptions.None);
                    var fieldLength = field.Length;
                    if (fieldLength != 2)
                        continue;
                    field = field.Select(i => i.Trim()).ToArray();
                    var key = field[0].ToLowerInvariant();
                    var value = field[1];
                    switch (key)
                    {
                        case NameSection:
                            manifest.Add(NameSection, value);
                            break;

                        case PathSection:
                            manifest.Add(PathSection, value);
                            break;

                        case DescriptionSection:
                            manifest.Add(DescriptionSection, value);
                            break;

                        case VersionSection:
                            manifest.Add(VersionSection, value);
                            break;

                        case KernelVersionSection:
                            manifest.Add(KernelVersionSection, value);
                            break;

                        case AuthorSection:
                            manifest.Add(AuthorSection, value);
                            break;

                        case WebsiteSection:
                            manifest.Add(WebsiteSection, value);
                            break;

                        case TagsSection:
                            manifest.Add(TagsSection, value);
                            break;

                        case DependenciesSection:
                            manifest.Add(DependenciesSection, value);
                            break;

                        case RuntimeSection:
                            manifest.Add(RuntimeSection, value);
                            break;

                        case CategorySection:
                            manifest.Add(CategorySection, value);
                            break;

                        case FeatureDescriptionSection:
                            manifest.Add(FeatureDescriptionSection, value);
                            break;

                        case FeatureNameSection:
                            manifest.Add(FeatureNameSection, value);
                            break;

                        case PrioritySection:
                            manifest.Add(PrioritySection, value);
                            break;

                        case FeaturesSection:
                            manifest.Add(FeaturesSection, reader.ReadToEnd());
                            break;

                        default:
                            defaultAction(key, value);
                            break;
                    }
                }
            }

            return manifest;
        }

        private static IEnumerable<FeatureDescriptor> GetFeaturesForExtension(IDictionary<string, string> manifest, ExtensionDescriptorEntry entry)
        {
            var featureDescriptors = new List<FeatureDescriptor>();

            var descriptor = entry.Descriptor;
            //默认特性
            var defaultFeature = new FeatureDescriptor
            {
                Id = entry.Id,
                Name = GetValue(manifest, FeatureNameSection) ?? descriptor.Name,
                Priority = GetValue(manifest, PrioritySection) != null ? int.Parse(GetValue(manifest, PrioritySection)) : 0,
                Description = GetValue(manifest, FeatureDescriptionSection) ?? GetValue(manifest, DescriptionSection) ?? string.Empty,
                Dependencies = ParseFeatureDependenciesEntry(GetValue(manifest, DependenciesSection)),
                Extension = entry,
                Category = GetValue(manifest, CategorySection)
            };

            featureDescriptors.Add(defaultFeature);

            //剩余特性
            var featuresText = GetValue(manifest, FeaturesSection);
            if (string.IsNullOrWhiteSpace(featuresText))
                return featureDescriptors;
            FeatureDescriptor featureDescriptor = null;
            using (var reader = new StringReader(featuresText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (IsFeatureDeclaration(line))
                    {
                        if (featureDescriptor != null)
                        {
                            if (!featureDescriptor.Equals(defaultFeature))
                            {
                                featureDescriptors.Add(featureDescriptor);
                            }
                        }

                        var featureDeclaration = line.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        var featureDescriptorId = featureDeclaration[0].Trim();
                        if (string.Equals(featureDescriptorId, entry.Id, StringComparison.OrdinalIgnoreCase))
                        {
                            featureDescriptor = defaultFeature;
                            featureDescriptor.Name = descriptor.Name;
                        }
                        else
                        {
                            featureDescriptor = new FeatureDescriptor
                            {
                                Id = featureDescriptorId,
                                Extension = entry
                            };
                        }
                    }
                    else if (IsFeatureFieldDeclaration(line))
                    {
                        if (featureDescriptor != null)
                        {
                            var featureField = line.Split(new[] { ":" }, 2, StringSplitOptions.None);
                            var featureFieldLength = featureField.Length;
                            if (featureFieldLength != 2)
                                continue;
                            for (var i = 0; i < featureFieldLength; i++)
                            {
                                featureField[i] = featureField[i].Trim();
                            }

                            switch (featureField[0].ToLowerInvariant())
                            {
                                case NameSection:
                                    featureDescriptor.Name = featureField[1];
                                    break;

                                case DescriptionSection:
                                    featureDescriptor.Description = featureField[1];
                                    break;

                                case CategorySection:
                                    featureDescriptor.Category = featureField[1];
                                    break;

                                case PrioritySection:
                                    featureDescriptor.Priority = int.Parse(featureField[1]);
                                    break;

                                case DependenciesSection:
                                    featureDescriptor.Dependencies = ParseFeatureDependenciesEntry(featureField[1]);
                                    break;
                            }
                        }
                        else
                        {
                            var message = string.Format("行 {0} 在清单文件中被忽略，来自扩展 {1}", line, entry.Id);
                            throw new ArgumentException(message);
                        }
                    }
                    else
                    {
                        var message = string.Format("行 {0} 在清单文件中被忽略，来自扩展 {1}", line, entry.Id);
                        throw new ArgumentException(message);
                    }
                }

                if (featureDescriptor != null && !featureDescriptor.Equals(defaultFeature))
                    featureDescriptors.Add(featureDescriptor);
            }

            return featureDescriptors;
        }

        private static bool IsFeatureFieldDeclaration(string line)
        {
            return line.StartsWith("\t\t") ||
                   line.StartsWith("\t    ") ||
                   line.StartsWith("    ") ||
                   line.StartsWith("    \t");
        }

        private static bool IsFeatureDeclaration(string line)
        {
            var lineLength = line.Length;
            if (line.StartsWith("\t") && lineLength >= 2)
                return !Char.IsWhiteSpace(line[1]);
            if (line.StartsWith("    ") && lineLength >= 5)
                return !Char.IsWhiteSpace(line[4]);

            return false;
        }

        private static IEnumerable<string> ParseFeatureDependenciesEntry(string dependenciesEntry)
        {
            return string.IsNullOrEmpty(dependenciesEntry) ? Enumerable.Empty<string>() : dependenciesEntry.Split(',').Select(s => s.Trim()).ToArray();
        }

        private static string GetValue(IDictionary<string, string> fields, string key)
        {
            string value;
            return fields.TryGetValue(key, out value) ? value : null;
        }

        #endregion Private Method
    }
}