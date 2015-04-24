using Autofac;
using Autofac.Core;
using Microsoft.CSharp.RuntimeBinder;
using Rabbit.Web.Mvc.DisplayManagement.Implementation;
using Rabbit.Web.Mvc.Mvc.Spooling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors.ShapeAttributeStrategy
{
    internal class ShapeAttributeBindingStrategy : IShapeTableProvider
    {
        private readonly IEnumerable<ShapeAttributeOccurrence> _shapeAttributeOccurrences;
        private readonly IComponentContext _componentContext;
        private readonly RouteCollection _routeCollection;

        public ShapeAttributeBindingStrategy(
            IEnumerable<ShapeAttributeOccurrence> shapeAttributeOccurrences,
            IComponentContext componentContext,
            RouteCollection routeCollection)
        {
            _shapeAttributeOccurrences = shapeAttributeOccurrences;
            // todo: 使用组件上下文将无法正常工作时，这是单例
            _componentContext = componentContext;
            _routeCollection = routeCollection;
        }

        public void Discover(ShapeTableBuilder builder)
        {
            foreach (var iter in _shapeAttributeOccurrences)
            {
                var occurrence = iter;
                var shapeType = occurrence.ShapeAttribute.ShapeType ?? occurrence.MethodInfo.Name;
                builder.Describe(shapeType)
                    .From(occurrence.Feature)
                    .BoundAs(
                        occurrence.MethodInfo.DeclaringType.FullName + "::" + occurrence.MethodInfo.Name,
                        descriptor => CreateDelegate(occurrence));
            }
        }

        [DebuggerStepThrough]
        private Func<DisplayContext, IHtmlString> CreateDelegate(
            ShapeAttributeOccurrence attributeOccurrence)
        {
            return context =>
            {
                var serviceInstance = _componentContext.ResolveComponent(attributeOccurrence.Registration, Enumerable.Empty<Parameter>());

                return PerformInvoke(context, attributeOccurrence.MethodInfo, serviceInstance);
            };
        }

        private IHtmlString PerformInvoke(DisplayContext displayContext, MethodInfo methodInfo, object serviceInstance)
        {
            var output = new HtmlStringWriter();
            var arguments = methodInfo.GetParameters()
                .Select(parameter => BindParameter(displayContext, parameter, output));

            var returnValue = methodInfo.Invoke(serviceInstance, arguments.ToArray());
            if (methodInfo.ReturnType != typeof(void))
            {
                output.Write(CoerceHtmlString(returnValue));
            }
            return output;
        }

        private static IHtmlString CoerceHtmlString(object invoke)
        {
            return invoke as IHtmlString ?? (invoke != null ? new HtmlString(invoke.ToString()) : null);
        }

        private object BindParameter(DisplayContext displayContext, ParameterInfo parameter, TextWriter output)
        {
            if (parameter.Name == "Shape")
                return displayContext.Value;

            if (parameter.Name == "Display")
                return displayContext.Display;

            if (parameter.Name == "Output" && parameter.ParameterType == typeof(TextWriter))
                return output;

            if (parameter.Name == "Output" && parameter.ParameterType == typeof(Action<object>))
                return new Action<object>(output.Write);

            if (parameter.Name == "Html")
            {
                return new HtmlHelper(
                    displayContext.ViewContext,
                    displayContext.ViewDataContainer,
                    _routeCollection);
            }

            if (parameter.Name == "Url" && parameter.ParameterType.IsAssignableFrom(typeof(UrlHelper)))
            {
                return new UrlHelper(displayContext.ViewContext.RequestContext, _routeCollection);
            }

            var getter = Getters.GetOrAdd(parameter.Name, n =>
                CallSite<Func<CallSite, object, dynamic>>.Create(
                Microsoft.CSharp.RuntimeBinder.Binder.GetMember(
                CSharpBinderFlags.None, n, null, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) })));

            var result = getter.Target(getter, displayContext.Value);

            if (result == null)
                return null;

            var converter = Converters.GetOrAdd(parameter.ParameterType, CompileConverter);
            var argument = converter.Invoke(result);
            return argument;
        }

        private static readonly ConcurrentDictionary<string, CallSite<Func<CallSite, object, dynamic>>> Getters =
            new ConcurrentDictionary<string, CallSite<Func<CallSite, object, dynamic>>>();

        private static readonly ConcurrentDictionary<Type, Func<dynamic, object>> Converters =
            new ConcurrentDictionary<Type, Func<dynamic, object>>();

        private static Func<dynamic, object> CompileConverter(Type targetType)
        {
            var valueParameter = Expression.Parameter(typeof(object), "value");

            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.Dynamic(
                        Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, targetType, null),
                        targetType,
                        valueParameter),
                    typeof(object)),
                valueParameter).Compile();
        }
    }
}