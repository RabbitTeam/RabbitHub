using Microsoft.CSharp.RuntimeBinder;
using Rabbit.Kernel;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using Rabbit.Kernel.Works;
using Rabbit.Web.Mvc.DisplayManagement.Descriptors;
using Rabbit.Web.Mvc.DisplayManagement.Shapes;
using Rabbit.Web.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;

namespace Rabbit.Web.Mvc.DisplayManagement.Implementation
{
    internal sealed class DefaultDisplayManager : IDisplayManager
    {
        private readonly Lazy<IShapeTableLocator> _shapeTableLocator;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IEnumerable<IShapeDisplayEvents> _shapeDisplayEvents;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly CallSite<Func<CallSite, object, Shape>> ConvertAsShapeCallsite = CallSite<Func<CallSite, object, Shape>>.Create(
                new ForgivingConvertBinder(
                    (ConvertBinder)Binder.Convert(
                    CSharpBinderFlags.ConvertExplicit,
                    typeof(Shape),
                    null)));

        public DefaultDisplayManager(
            IWorkContextAccessor workContextAccessor,
            IEnumerable<IShapeDisplayEvents> shapeDisplayEvents,
            IHttpContextAccessor httpContextAccessor,
            Lazy<IShapeTableLocator> shapeTableLocator)
        {
            _shapeTableLocator = shapeTableLocator;
            _workContextAccessor = workContextAccessor;
            _shapeDisplayEvents = shapeDisplayEvents;
            _httpContextAccessor = httpContextAccessor;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public IHtmlString Execute(DisplayContext context)
        {
            var shape = ConvertAsShapeCallsite.Target(ConvertAsShapeCallsite, context.Value);

            if (shape == null)
                return CoerceHtmlString(context.Value);

            var shapeMetadata = shape.Metadata;
            if (shapeMetadata == null || string.IsNullOrEmpty(shapeMetadata.Type))
                return CoerceHtmlString(context.Value);

            var workContext = _workContextAccessor.GetContext();
            var shapeTable = _httpContextAccessor.Current() != null
                ? _shapeTableLocator.Value.Lookup(workContext.GetCurrentTheme().Id)
                : _shapeTableLocator.Value.Lookup(null);

            var displayingContext = new ShapeDisplayingContext
            {
                Shape = shape,
                ShapeMetadata = shapeMetadata
            };
            _shapeDisplayEvents.Invoke(sde => sde.Displaying(displayingContext), Logger);

            ShapeBinding shapeBinding;
            if (TryGetDescriptorBinding(shapeMetadata.Type, Enumerable.Empty<string>(), shapeTable, out shapeBinding))
            {
                shapeBinding.ShapeDescriptor.Displaying.Invoke(action => action(displayingContext), Logger);

                shapeMetadata.BindingSources = shapeBinding.ShapeDescriptor.BindingSources.Where(x => x != null).ToList();
                if (!shapeMetadata.BindingSources.Any())
                {
                    shapeMetadata.BindingSources.Add(shapeBinding.ShapeDescriptor.BindingSource);
                }
            }

            shapeMetadata.Displaying.Invoke(action => action(displayingContext), Logger);

            if (displayingContext.ChildContent != null)
            {
                shape.Metadata.ChildContent = displayingContext.ChildContent;
            }
            else
            {
                ShapeBinding actualBinding;
                if (TryGetDescriptorBinding(shapeMetadata.Type, shapeMetadata.Alternates, shapeTable, out actualBinding))
                {
                    shape.Metadata.ChildContent = Process(actualBinding, shape, context);
                }
                else
                {
                    throw new RabbitException(T("找不到形状类型 {0}", shapeMetadata.Type));
                }
            }

            foreach (var frameType in shape.Metadata.Wrappers)
            {
                ShapeBinding frameBinding;
                if (TryGetDescriptorBinding(frameType, Enumerable.Empty<string>(), shapeTable, out frameBinding))
                {
                    shape.Metadata.ChildContent = Process(frameBinding, shape, context);
                }
            }

            var displayedContext = new ShapeDisplayedContext
            {
                Shape = shape,
                ShapeMetadata = shape.Metadata,
                ChildContent = shape.Metadata.ChildContent,
            };

            _shapeDisplayEvents.Invoke(sde =>
            {
                var prior = displayedContext.ChildContent = displayedContext.ShapeMetadata.ChildContent;
                sde.Displayed(displayedContext);
                if (prior != displayedContext.ChildContent)
                    displayedContext.ShapeMetadata.ChildContent = displayedContext.ChildContent;
            }, Logger);

            if (shapeBinding != null)
            {
                shapeBinding.ShapeDescriptor.Displayed.Invoke(action =>
                {
                    var prior = displayedContext.ChildContent = displayedContext.ShapeMetadata.ChildContent;
                    action(displayedContext);
                    if (prior != displayedContext.ChildContent)
                        displayedContext.ShapeMetadata.ChildContent = displayedContext.ChildContent;
                }, Logger);
            }

            shapeMetadata.Displayed.Invoke(action => action(displayedContext), Logger);

            return shape.Metadata.ChildContent;
        }

        private static bool TryGetDescriptorBinding(string shapeType, IEnumerable<string> shapeAlternates, ShapeTable shapeTable, out ShapeBinding shapeBinding)
        {
            foreach (var shapeAlternate in shapeAlternates.Reverse())
            {
                if (shapeTable.Bindings.TryGetValue(shapeAlternate, out shapeBinding))
                {
                    return true;
                }
            }

            var shapeTypeScan = shapeType;
            for (; ; )
            {
                if (shapeTable.Bindings.TryGetValue(shapeTypeScan, out shapeBinding))
                {
                    return true;
                }

                var delimiterIndex = shapeTypeScan.LastIndexOf("__", StringComparison.Ordinal);
                if (delimiterIndex < 0)
                {
                    return false;
                }

                shapeTypeScan = shapeTypeScan.Substring(0, delimiterIndex);
            }
        }

        private static IHtmlString CoerceHtmlString(object value)
        {
            if (value == null)
                return null;

            var result = value as IHtmlString;
            return result ?? new HtmlString(HttpUtility.HtmlEncode(value));
        }

        private static IHtmlString Process(ShapeBinding shapeBinding, IShape shape, DisplayContext context)
        {
            if (shapeBinding == null || shapeBinding.Binding == null)
            {
                return shape.Metadata.ChildContent ?? new HtmlString(string.Empty);
            }
            return CoerceHtmlString(shapeBinding.Binding(context));
        }

        private class ForgivingConvertBinder : ConvertBinder
        {
            private readonly ConvertBinder _innerBinder;

            public ForgivingConvertBinder(ConvertBinder innerBinder)
                : base(innerBinder.ReturnType, innerBinder.Explicit)
            {
                _innerBinder = innerBinder;
            }

            public override DynamicMetaObject FallbackConvert(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                var result = _innerBinder.FallbackConvert(
                    target,
                    errorSuggestion ?? new DynamicMetaObject(Expression.Default(_innerBinder.ReturnType), GetTypeRestriction(target)));
                return result;
            }

            private static BindingRestrictions GetTypeRestriction(DynamicMetaObject obj)
            {
                if ((obj.Value == null) && obj.HasValue)
                {
                    return BindingRestrictions.GetInstanceRestriction(obj.Expression, null);
                }
                return BindingRestrictions.GetTypeRestriction(obj.Expression, obj.LimitType);
            }
        }
    }
}