using Rabbit.Web.Mvc.DisplayManagement.Descriptors;
using Rabbit.Web.Mvc.DisplayManagement.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Web.Mvc.DisplayManagement.Implementation
{
    internal sealed class DefaultShapeFactory : Composite, IShapeFactory
    {
        private readonly IEnumerable<Lazy<IShapeFactoryEvents>> _events;
        private readonly Lazy<IShapeTableLocator> _shapeTableLocator;

        public DefaultShapeFactory(
            IEnumerable<Lazy<IShapeFactoryEvents>> events,
            Lazy<IShapeTableLocator> shapeTableLocator)
        {
            _events = events;
            _shapeTableLocator = shapeTableLocator;
        }

        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Create(binder.Name, Arguments.From(args, binder.CallInfo.ArgumentNames));
            return true;
        }

        public IShape Create(string shapeType)
        {
            return Create(shapeType, Arguments.Empty(), () => new Shape());
        }

        public IShape Create(string shapeType, INamedEnumerable<object> parameters)
        {
            return Create(shapeType, parameters, () => new Shape());
        }

        public IShape Create(string shapeType, INamedEnumerable<object> parameters, Func<dynamic> createShape)
        {
            var defaultShapeTable = _shapeTableLocator.Value.Lookup(null);
            ShapeDescriptor shapeDescriptor;
            defaultShapeTable.Descriptors.TryGetValue(shapeType, out shapeDescriptor);

            parameters = parameters ?? Arguments.Empty();

            var creatingContext = new ShapeCreatingContext
            {
                New = this,
                ShapeFactory = this,
                ShapeType = shapeType,
                OnCreated = new List<Action<ShapeCreatedContext>>()
            };

            IEnumerable<object> positional = parameters.Positional.ToList();
            var baseType = positional.FirstOrDefault() as Type;

            if (baseType == null)
            {
                creatingContext.Create = createShape ?? (() => new Shape());
            }
            else
            {
                positional = positional.Skip(1);
                creatingContext.Create = () => Activator.CreateInstance(baseType);
            }

            foreach (var ev in _events)
            {
                ev.Value.Creating(creatingContext);
            }
            if (shapeDescriptor != null)
            {
                foreach (var ev in shapeDescriptor.Creating)
                {
                    ev(creatingContext);
                }
            }

            var createdContext = new ShapeCreatedContext
            {
                New = creatingContext.New,
                ShapeType = creatingContext.ShapeType,
                Shape = creatingContext.Create()
            };

            if (!(createdContext.Shape is IShape))
            {
                throw new InvalidOperationException("无限的形状基本类型: " + createdContext.Shape.GetType().ToString());
            }

            ShapeMetadata shapeMetadata = createdContext.Shape.Metadata;
            createdContext.Shape.Metadata.Type = shapeType;

            if (shapeDescriptor != null)
                shapeMetadata.Wrappers = shapeMetadata.Wrappers.Concat(shapeDescriptor.Wrappers).ToList();

            foreach (var ev in _events)
            {
                ev.Value.Created(createdContext);
            }
            if (shapeDescriptor != null)
            {
                foreach (var ev in shapeDescriptor.Created)
                {
                    ev(createdContext);
                }
            }
            foreach (var ev in creatingContext.OnCreated)
            {
                ev(createdContext);
            }

            var initializer = positional.SingleOrDefault();
            if (initializer != null)
            {
                foreach (var prop in initializer.GetType().GetProperties())
                {
                    createdContext.Shape[prop.Name] = prop.GetValue(initializer, null);
                }
            }

            foreach (var kv in parameters.Named)
            {
                createdContext.Shape[kv.Key] = kv.Value;
            }

            return createdContext.Shape;
        }
    }
}