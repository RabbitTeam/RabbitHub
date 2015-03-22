using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Indexed;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.ShellBuilders.Models;
using Rabbit.Kernel.Events;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Environment.ShellBuilders.Impl
{
    internal sealed class DefaultShellContainerFactory : IShellContainerFactory
    {
        #region Field

        private readonly ILifetimeScope _lifetimeScope;
        private readonly IEnumerable<IShellContainerRegistrations> _shellContainerRegistrationses;

        #endregion Field

        #region Constructor

        public DefaultShellContainerFactory(ILifetimeScope lifetimeScope, IEnumerable<IShellContainerRegistrations> shellContainerRegistrationses)
        {
            _lifetimeScope = lifetimeScope;
            _shellContainerRegistrationses = shellContainerRegistrationses;
        }

        #endregion Constructor

        #region Implementation of IShellContainerFactory

        /// <summary>
        /// 创建一个外壳容器。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <param name="blueprint">外壳蓝图。</param>
        /// <returns>外壳容器。</returns>
        public ILifetimeScope CreateContainer(ShellSettings settings, ShellBlueprint blueprint)
        {
            var intermediateScope = _lifetimeScope.BeginLifetimeScope(
                builder =>
                {
                    //TODO:CollectionOrderModule、CacheModule 等Module是公共的，需要验证 Root 范围注册了子级生命范围是否生效，如果生效则在外壳容器中忽略掉这些Module
                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IModule).IsAssignableFrom(t.Type)))
                    {
                        RegisterType(builder, item)
                            .Keyed<IModule>(item.Type)
                            .InstancePerDependency();
                    }
                });

            return intermediateScope.BeginLifetimeScope(
                "shell",
                builder =>
                {
                    builder.Register(ctx => settings);
                    builder.Register(ctx => blueprint.Descriptor);
                    builder.Register(ctx => blueprint);

                    var moduleIndex = intermediateScope.Resolve<IIndex<Type, IModule>>();
                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IModule).IsAssignableFrom(t.Type)))
                    {
                        builder.RegisterModule(moduleIndex[item.Type]);
                    }

                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IDependency).IsAssignableFrom(t.Type)))
                    {
                        var registration = RegisterType(builder, item)
                            .InstancePerLifetimeScope();

                        foreach (var interfaceType in item.Type.GetInterfaces()
                            .Where(itf => typeof(IDependency).IsAssignableFrom(itf)
                                      && !typeof(IEventHandler).IsAssignableFrom(itf)))
                        {
                            registration = registration.As(interfaceType);
                            if (typeof(ISingletonDependency).IsAssignableFrom(interfaceType))
                            {
                                registration = registration.InstancePerMatchingLifetimeScope("shell");
                            }
                            else if (typeof(IUnitOfWorkDependency).IsAssignableFrom(interfaceType))
                            {
                                registration = registration.InstancePerMatchingLifetimeScope("work");
                            }
                            else if (typeof(ITransientDependency).IsAssignableFrom(interfaceType))
                            {
                                registration = registration.InstancePerDependency();
                            }
                        }

                        if (!typeof(IEventHandler).IsAssignableFrom(item.Type))
                            continue;
                        var interfaces = item.Type.GetInterfaces();
                        foreach (var interfaceType in interfaces)
                        {
                            if (interfaceType.GetInterface(typeof(IEventHandler).Name) != null)
                            {
                                registration = registration.Named<IEventHandler>(interfaceType.Name);
                            }
                        }
                    }

                    _shellContainerRegistrationses.Invoke(i => i.Registrations(builder, blueprint), NullLogger.Instance);
                });
        }

        #endregion Implementation of IShellContainerFactory

        #region Private Method

        private IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(ContainerBuilder builder, BlueprintItem item)
        {
            return builder.RegisterType(item.Type)
                .WithProperty("Feature", item.Feature)
                .WithMetadata("Feature", item.Feature);
        }

        #endregion Private Method
    }
}