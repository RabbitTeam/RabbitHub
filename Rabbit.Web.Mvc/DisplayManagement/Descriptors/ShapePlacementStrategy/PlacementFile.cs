using System.Collections.Generic;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors.ShapePlacementStrategy
{
    internal class PlacementFile : PlacementNode
    {
    }

    internal class PlacementNode
    {
        public IEnumerable<PlacementNode> Nodes { get; set; }
    }

    internal class PlacementMatch : PlacementNode
    {
        public IDictionary<string, string> Terms { get; set; }
    }

    internal class PlacementShapeLocation : PlacementNode
    {
        public string ShapeType { get; set; }

        public string Location { get; set; }
    }
}