using Autofac.Core;
using Rabbit.Kernel.Extensions.Models;
using System;
using System.Reflection;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors.ShapeAttributeStrategy
{
    internal sealed class ShapeAttributeOccurrence
    {
        private readonly Func<Feature> _feature;

        public ShapeAttributeOccurrence(ShapeAttribute shapeAttribute, MethodInfo methodInfo, IComponentRegistration registration, Func<Feature> feature)
        {
            ShapeAttribute = shapeAttribute;
            MethodInfo = methodInfo;
            Registration = registration;
            _feature = feature;
        }

        public ShapeAttribute ShapeAttribute { get; private set; }

        public MethodInfo MethodInfo { get; private set; }

        public IComponentRegistration Registration { get; private set; }

        public Feature Feature { get { return _feature(); } }
    }
}