using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Web.Mvc.Utility.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors.ShapePlacementStrategy
{
    /// <summary>
    /// 此组件发现并宣布由Placement.info文件的内容所隐含的形状改变
    /// </summary>
    internal sealed class ShapePlacementParsingStrategy : IShapeTableProvider
    {
        private readonly IExtensionManager _extensionManager;
        private readonly ShellDescriptor _shellDescriptor;
        private readonly IPlacementFileParser _placementFileParser;

        public ShapePlacementParsingStrategy(
            IExtensionManager extensionManager,
            ShellDescriptor shellDescriptor,
            IPlacementFileParser placementFileParser)
        {
            _extensionManager = extensionManager;
            _shellDescriptor = shellDescriptor;
            _placementFileParser = placementFileParser;
        }

        public void Discover(ShapeTableBuilder builder)
        {
            var availableFeatures = _extensionManager.AvailableFeatures();
            var activeFeatures = availableFeatures.Where(fd => FeatureIsTheme(fd) || FeatureIsEnabled(fd));
            var activeExtensions = Once(activeFeatures);

            foreach (var featureDescriptor in activeExtensions.SelectMany(extensionDescriptor => extensionDescriptor.Descriptor.Features.Where(fd => fd.Id == fd.Extension.Id)))
            {
                ProcessFeatureDescriptor(builder, featureDescriptor);
            }
        }

        private void ProcessFeatureDescriptor(ShapeTableBuilder builder, FeatureDescriptor featureDescriptor)
        {
            var virtualPath = featureDescriptor.Extension.Location + "/" + featureDescriptor.Extension.Id + "/Placement.info";
            var placementFile = _placementFileParser.Parse(virtualPath);
            if (placementFile != null)
            {
                ProcessPlacementFile(builder, featureDescriptor, placementFile);
            }
        }

        private static void ProcessPlacementFile(ShapeTableBuilder builder, FeatureDescriptor featureDescriptor, PlacementFile placementFile)
        {
            var feature = new Feature { Descriptor = featureDescriptor };

            //反转树成叶子的列表，并在堆栈
            var entries = DrillDownShapeLocations(placementFile.Nodes, Enumerable.Empty<PlacementMatch>());
            foreach (var entry in entries)
            {
                var shapeLocation = entry.Item1;
                var matches = entry.Item2.ToArray();

                string shapeType;
                string differentiator;
                GetShapeType(shapeLocation, out shapeType, out differentiator);

                Func<ShapePlacementContext, bool> predicate = ctx => true;
                if (differentiator != string.Empty)
                {
                    predicate = ctx => (ctx.Differentiator ?? string.Empty) == differentiator;
                }

                if (matches.Any())
                {
                    predicate = matches.SelectMany(match => match.Terms).Aggregate(predicate, BuildPredicate);
                }

                var placement = new PlacementInfo();

                var segments = shapeLocation.Location.Split(';').Select(s => s.Trim());
                foreach (var segment in segments)
                {
                    if (!segment.Contains('='))
                    {
                        placement.Location = segment;
                    }
                    else
                    {
                        var index = segment.IndexOf('=');
                        var property = segment.Substring(0, index).ToLower();
                        var value = segment.Substring(index + 1);
                        switch (property)
                        {
                            case "shape":
                                placement.ShapeType = value;
                                break;

                            case "alternate":
                                placement.Alternates = new[] { value };
                                break;

                            case "wrapper":
                                placement.Wrappers = new[] { value };
                                break;
                        }
                    }
                }

                builder.Describe(shapeType)
                    .From(feature)
                    .Placement(ctx =>
                    {
                        var hit = predicate(ctx);
                        //产生'调试'信息跟踪哪个文件起源的实际位置
                        if (hit)
                        {
                            var virtualPath = featureDescriptor.Extension.Location + "/" + featureDescriptor.Extension.Id + "/Placement.info";
                            ctx.Source = virtualPath;
                        }
                        return hit;
                    }, placement);
            }
        }

        private static void GetShapeType(PlacementShapeLocation shapeLocation, out string shapeType, out string differentiator)
        {
            differentiator = string.Empty;
            shapeType = shapeLocation.ShapeType;
            var dashIndex = shapeType.LastIndexOf('-');
            if (dashIndex <= 0 || dashIndex >= shapeType.Length - 1)
                return;
            differentiator = shapeType.Substring(dashIndex + 1);
            shapeType = shapeType.Substring(0, dashIndex);
        }

        public static Func<ShapePlacementContext, bool> BuildPredicate(Func<ShapePlacementContext, bool> predicate, KeyValuePair<string, string> term)
        {
            var expression = term.Value;
            switch (term.Key)
            {
                /*                case "ContentPart":
                                    return ctx => ctx.Content != null
                                        //                        && ctx.Content.ContentItem.Parts.Any(part => part.PartDefinition.Name == expression)
                                        && predicate(ctx);*/

                case "ContentType":
                    if (expression.EndsWith("*"))
                    {
                        var prefix = expression.Substring(0, expression.Length - 1);
                        return ctx => ((ctx.ContentType ?? string.Empty).StartsWith(prefix) || (ctx.Stereotype ?? string.Empty).StartsWith(prefix)) && predicate(ctx);
                    }
                    return ctx => ((ctx.ContentType == expression) || (ctx.Stereotype == expression)) && predicate(ctx);

                case "DisplayType":
                    if (expression.EndsWith("*"))
                    {
                        var prefix = expression.Substring(0, expression.Length - 1);
                        return ctx => (ctx.DisplayType ?? string.Empty).StartsWith(prefix) && predicate(ctx);
                    }
                    return ctx => (ctx.DisplayType == expression) && predicate(ctx);

                case "Path":
                    var normalizedPath = VirtualPathUtility.IsAbsolute(expression)
                                             ? VirtualPathUtility.ToAppRelative(expression)
                                             : VirtualPathUtility.Combine("~/", expression);

                    if (normalizedPath.EndsWith("*"))
                    {
                        var prefix = normalizedPath.Substring(0, normalizedPath.Length - 1);
                        return ctx => VirtualPathUtility.ToAppRelative(String.IsNullOrEmpty(ctx.Path) ? "/" : ctx.Path).StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && predicate(ctx);
                    }

                    normalizedPath = VirtualPathUtility.AppendTrailingSlash(normalizedPath);
                    return ctx => (ctx.Path.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase)) && predicate(ctx);
            }
            return predicate;
        }

        private static IEnumerable<Tuple<PlacementShapeLocation, IEnumerable<PlacementMatch>>> DrillDownShapeLocations(
            IEnumerable<PlacementNode> nodes,
            IEnumerable<PlacementMatch> path)
        {
            nodes = nodes.ToArray();
            path = path.ToArray();
            //返回形状位置的节点在这个地方
            foreach (var placementShapeLocation in nodes.OfType<PlacementShapeLocation>())
            {
                yield return new Tuple<PlacementShapeLocation, IEnumerable<PlacementMatch>>(placementShapeLocation, path);
            }
            //递归分解成匹配的节点
            foreach (var placementMatch in nodes.OfType<PlacementMatch>())
            {
                foreach (var findShapeLocation in DrillDownShapeLocations(placementMatch.Nodes, path.Concat(new[] { placementMatch })))
                {
                    yield return findShapeLocation;
                }
            }
        }

        private static bool FeatureIsTheme(FeatureDescriptor fd)
        {
            return DefaultExtensionTypes.IsTheme(fd.Extension.ExtensionType);
        }

        private bool FeatureIsEnabled(FeatureDescriptor fd)
        {
            return _shellDescriptor.Features.Any(sf => sf.Name == fd.Id);
        }

        private static IEnumerable<ExtensionDescriptorEntry> Once(IEnumerable<FeatureDescriptor> featureDescriptors)
        {
            var once = new ConcurrentDictionary<string, object>();
            return featureDescriptors.Select(fd => fd.Extension).Where(ed => once.TryAdd(ed.Id, null)).ToList();
        }
    }
}