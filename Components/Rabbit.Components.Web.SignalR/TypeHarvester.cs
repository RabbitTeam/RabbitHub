using Rabbit.Kernel;
using Rabbit.Kernel.Environment.ShellBuilders.Models;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Web.SignalR
{
    internal interface ITypeHarvester : ISingletonDependency
    {
        IEnumerable<Tuple<Type, Feature>> Get(Func<Type, bool> predicate);

        IEnumerable<Tuple<Type, Feature>> Get(Type type);

        IEnumerable<Tuple<Type, Feature>> Get<T>();
    }

    internal sealed class DefaultTypeHarvester : ITypeHarvester
    {
        private readonly IExtensionManager _extensions;
        private readonly ShellBlueprint _shell;

        public DefaultTypeHarvester(IExtensionManager extensions, ShellBlueprint shell)
        {
            _extensions = extensions;
            _shell = shell;
        }

        #region Implementation of ITypeHarvester

        public IEnumerable<Tuple<Type, Feature>> Get(Func<Type, bool> predicate)
        {
            return _extensions
                .LoadFeatures(_extensions.EnabledFeatures(_shell.Descriptor))
                .SelectMany(feature => feature.ExportedTypes
                    .Where(predicate)
                    .Select(c => new Tuple<Type, Feature>(c, feature)));
        }

        public IEnumerable<Tuple<Type, Feature>> Get(Type type)
        {
            return Get(type.IsAssignableFrom);
        }

        public IEnumerable<Tuple<Type, Feature>> Get<T>()
        {
            return Get(typeof(T));
        }

        #endregion Implementation of ITypeHarvester
    }
}