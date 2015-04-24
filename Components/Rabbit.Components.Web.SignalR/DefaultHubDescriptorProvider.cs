using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rabbit.Components.Web.SignalR
{
    internal sealed class DefaultHubDescriptorProvider : IHubDescriptorProvider
    {
        #region Field

        private readonly ITypeHarvester _typeHarvester;
        private readonly Lazy<IDictionary<string, HubDescriptor>> _hubs;

        #endregion Field

        #region Constructor

        public DefaultHubDescriptorProvider(ITypeHarvester typeHarvester)
        {
            _typeHarvester = typeHarvester;
            _hubs = new Lazy<IDictionary<string, HubDescriptor>>(BuildHubsCache);
        }

        #endregion Constructor

        #region Implementation of IHubDescriptorProvider

        public IList<HubDescriptor> GetHubs()
        {
            return _hubs.Value
                .Select(kv => kv.Value)
                .Distinct()
                .ToList();
        }

        public bool TryGetHub(string hubName, out HubDescriptor descriptor)
        {
            return _hubs.Value.TryGetValue(hubName, out descriptor);
        }

        #endregion Implementation of IHubDescriptorProvider

        #region Private Method

        private IDictionary<string, HubDescriptor> BuildHubsCache()
        {
            var types = _typeHarvester.Get(IsHubType).Select(tt => tt.Item1);

            var cacheEntries = types
                .Select(type => new HubDescriptor
                {
                    NameSpecified = (GetHubAttributeName(type) != null),
                    Name = GetHubName(type),
                    HubType = type
                })
                .ToDictionary(hub => hub.Name,
                              hub => hub,
                              StringComparer.OrdinalIgnoreCase);

            return cacheEntries;
        }

        private static bool IsHubType(Type type)
        {
            try
            {
                return typeof(IHub).IsAssignableFrom(type) &&
                       !type.IsAbstract &&
                       (type.Attributes.HasFlag(TypeAttributes.Public) ||
                        type.Attributes.HasFlag(TypeAttributes.NestedPublic));
            }
            catch
            {
                return false;
            }
        }

        private static string GetHubAttributeName(ICustomAttributeProvider type)
        {
            return ReflectionHelper.GetAttributeValue<HubNameAttribute, string>(type, attr => attr.HubName);
        }

        private static string GetHubName(MemberInfo type)
        {
            return GetHubAttributeName(type) ?? type.Name;
        }

        #endregion Private Method
    }
}