using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Module = Autofac.Module;

namespace Rabbit.Kernel.Environment
{
    internal sealed class HostComponentsConfigModule : Module
    {
        #region Field

        private readonly IDictionary<string, IEnumerable<PropertyEntry>> _config = new Dictionary<string, IEnumerable<PropertyEntry>>();

        #endregion Field

        #region Constructor

        public HostComponentsConfigModule()
        {
            //由框架调用这个类是一个 "Autofac模块"
        }

        public HostComponentsConfigModule(string fileName)
        {
            var doc = XDocument.Load(fileName);
            foreach (var component in doc.Elements(XNames.HostComponents).Elements(XNames.Components).Elements(XNames.Component))
            {
                var componentType = Attr(component, XNames.Type);
                if (componentType == null)
                    continue;

                var properties = component
                    .Elements(XNames.Properties)
                    .Elements(XNames.Property)
                    .Select(property => new PropertyEntry { Name = Attr(property, XNames.Name), Value = Attr(property, XNames.Value) })
                    .Where(t => !string.IsNullOrEmpty(t.Name) && !string.IsNullOrEmpty(t.Value))
                    .ToList();

                if (!properties.Any())
                    continue;

                _config.Add(componentType, properties);
            }
        }

        #endregion Constructor

        #region Overrides of Module

        /// <summary>
        /// Override to attach module-specific functionality to a
        ///             component registration.
        /// </summary>
        /// <remarks>
        /// This method will be called for all existing <i>and future</i> component
        ///             registrations - ordering is not important.
        /// </remarks>
        /// <param name="componentRegistry">The component registry.</param><param name="registration">The registration to attach functionality to.</param>
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var implementationType = registration.Activator.LimitType;

            IEnumerable<PropertyEntry> properties;
            if (!_config.TryGetValue(implementationType.FullName, out properties))
                return;

            var injectors = BuildPropertiesInjectors(implementationType, properties).ToArray();

            if (!injectors.Any())
                return;

            registration.Activated += (s, e) =>
            {
                foreach (var injector in injectors)
                    injector(e.Context, e.Instance);
            };
        }

        #endregion Overrides of Module

        #region Private Method

        private static bool ChangeToCompatibleType(string value, Type destinationType, out object result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = null;
                return false;
            }

            if (destinationType.IsInstanceOfType(value))
            {
                result = value;
                return true;
            }

            try
            {
                result = TypeDescriptor.GetConverter(destinationType).ConvertFrom(value);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private static string Attr(XElement component, XName name)
        {
            var attr = component.Attribute(name);
            return attr == null ? null : attr.Value;
        }

        private static IEnumerable<Action<IComponentContext, object>> BuildPropertiesInjectors(IReflect componentType, IEnumerable<PropertyEntry> properties)
        {
            var settableProperties = componentType
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new
                {
                    PropertyInfo = p,
                    IndexParameters = p.GetIndexParameters(),
                    Accessors = p.GetAccessors(false),
                    PropertyEntry = properties.FirstOrDefault(t => t.Name == p.Name)
                })
                .Where(x => x.PropertyEntry != null)
                .Where(x => !x.IndexParameters.Any())
                .Where(x => x.Accessors.Length != 1 || x.Accessors[0].ReturnType == typeof(void));

            foreach (var entry in settableProperties)
            {
                var propertyInfo = entry.PropertyInfo;
                var propertyEntry = entry.PropertyEntry;

                yield return (ctx, instance) =>
                {
                    object value;
                    if (ChangeToCompatibleType(propertyEntry.Value, propertyInfo.PropertyType, out value))
                        propertyInfo.SetValue(instance, value, null);
                };
            }
        }

        #endregion Private Method

        #region Help Class

        private static class XNames
        {
            private const string Xmlns = "";
            public static readonly XName HostComponents = XName.Get("HostComponents", Xmlns);
            public static readonly XName Components = XName.Get("Components", Xmlns);
            public static readonly XName Component = XName.Get("Component", Xmlns);
            public static readonly XName Properties = XName.Get("Properties", Xmlns);
            public static readonly XName Property = XName.Get("Property", Xmlns);
            public static readonly XName Type = XName.Get("Type");
            public static readonly XName Name = XName.Get("Name");
            public static readonly XName Value = XName.Get("Value");
        }

        //组件类型名称=>[属性名称，属性值列表]
        private sealed class PropertyEntry
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        #endregion Help Class
    }
}