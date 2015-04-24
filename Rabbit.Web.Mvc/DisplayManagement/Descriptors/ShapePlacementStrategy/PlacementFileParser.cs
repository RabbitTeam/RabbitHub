using Rabbit.Kernel;
using Rabbit.Kernel.Caching;
using Rabbit.Kernel.FileSystems.Application;
using Rabbit.Kernel.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors.ShapePlacementStrategy
{
    internal interface IPlacementFileParser : IDependency
    {
        PlacementFile Parse(string virtualPath);
    }

    internal class PlacementFileParser : IPlacementFileParser
    {
        private readonly ICacheManager _cacheManager;
        private readonly IApplicationFolder _webSiteFolder;

        public PlacementFileParser(ICacheManager cacheManager, IApplicationFolder webSiteFolder)
        {
            _cacheManager = cacheManager;
            _webSiteFolder = webSiteFolder;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool DisableMonitoring { get; set; }

        public PlacementFile Parse(string virtualPath)
        {
            return _cacheManager.Get(virtualPath, context =>
            {
                if (!DisableMonitoring)
                {
                    Logger.Debug("监控虚拟路径 \"{0}\"", virtualPath);
                    context.Monitor(_webSiteFolder.WhenPathChanges(virtualPath));
                }

                var placementText = _webSiteFolder.ReadFile(virtualPath);
                return ParseImplementation(placementText);
            });
        }

        private PlacementFile ParseImplementation(string placementText)
        {
            if (placementText == null)
                return null;

            var element = XElement.Parse(placementText);
            return new PlacementFile
            {
                Nodes = Accept(element).ToList()
            };
        }

        private IEnumerable<PlacementNode> Accept(XElement element)
        {
            switch (element.Name.LocalName)
            {
                case "Placement":
                    return AcceptMatch(element);

                case "Match":
                    return AcceptMatch(element);

                case "Place":
                    return AcceptPlace(element);
            }
            return Enumerable.Empty<PlacementNode>();
        }

        private IEnumerable<PlacementNode> AcceptMatch(XElement element)
        {
            if (element.HasAttributes == false)
            {
                //无属性的匹配就会崩溃孩子向上的结果，而不是返回一个无条件的节点
                return element.Elements().SelectMany(Accept);
            }

            //返回携带返回键/值条件的字典，并具有嵌套为节点的子规则匹配的节点
            return new[]{new PlacementMatch{
                Terms = element.Attributes().ToDictionary(attr=>attr.Name.LocalName, attr=>attr.Value),
                Nodes=element.Elements().SelectMany(Accept).ToArray(),
            }};
        }

        private static IEnumerable<PlacementShapeLocation> AcceptPlace(XElement element)
        {
            //返回属性的部件位置
            return element.Attributes().Select(attr => new PlacementShapeLocation
            {
                ShapeType = attr.Name.LocalName,
                Location = attr.Value
            });
        }
    }
}