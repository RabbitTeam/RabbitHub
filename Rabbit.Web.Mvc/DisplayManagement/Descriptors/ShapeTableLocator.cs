using Rabbit.Kernel;
using System.Collections.Concurrent;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors
{
    internal interface IShapeTableLocator : IUnitOfWorkDependency
    {
        ShapeTable Lookup(string themeName);
    }

    internal class ShapeTableLocator : IShapeTableLocator
    {
        private readonly IShapeTableManager _shapeTableManager;
        private readonly ConcurrentDictionary<string, ShapeTable> _shapeTables = new ConcurrentDictionary<string, ShapeTable>();

        public ShapeTableLocator(IShapeTableManager shapeTableManager)
        {
            _shapeTableManager = shapeTableManager;
        }

        public ShapeTable Lookup(string themeName)
        {
            return _shapeTables.GetOrAdd(themeName ?? "", _ => _shapeTableManager.GetShapeTable(themeName));
        }
    }
}