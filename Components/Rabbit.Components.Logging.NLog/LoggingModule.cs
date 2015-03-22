using Autofac;
using Autofac.Core;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Module = Autofac.Module;

namespace Rabbit.Components.Logging.NLog
{
    internal sealed class LoggingModule : Module
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggerCache;

        public LoggingModule()
        {
            _loggerCache = new ConcurrentDictionary<string, ILogger>();
        }

        #region Overrides of Module

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be
        ///             registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoggingConfigurationResolve>().As<ILoggingConfigurationResolve>().SingleInstance();
            builder.RegisterType<LoggerFactory>().As<ILoggerFactory>().InstancePerLifetimeScope();
            builder.Register(CreateLogger).As<ILogger>().InstancePerDependency();
        }

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

            //生成记录器属性注入动作。
            var injectors = BuildLoggerInjectors(implementationType).ToArray();

            if (!injectors.Any())
                return;

            //该服务实例被激活完成时注入的记录器实例。
            registration.Activated += (s, e) =>
            {
                foreach (var injector in injectors)
                    injector(e.Context, e.Instance);
            };
        }

        #endregion Overrides of Module

        private IEnumerable<Action<IComponentContext, object>> BuildLoggerInjectors(IReflect componentType)
        {
            //寻找类型为 "ILogger" 并且具有set方法的属性。
            var loggerProperties = componentType
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new
                {
                    PropertyInfo = p,
                    p.PropertyType,
                    IndexParameters = p.GetIndexParameters(),
                    Accessors = p.GetAccessors(false)
                })
                .Where(x => x.PropertyType == typeof(ILogger)) //必须是一个日志记录器
                .Where(x => !x.IndexParameters.Any()) //没有索引器
                .Where(x => x.Accessors.Length != 1 || x.Accessors[0].ReturnType == typeof(void)); //必须具有set方法。

            return loggerProperties.Select(entry => entry.PropertyInfo).Select(propertyInfo => (Action<IComponentContext, object>)((ctx, instance) =>
            {
                var component = componentType.ToString();
                var logger = _loggerCache.GetOrAdd(component, key => ctx.Resolve<ILogger>(new TypedParameter(typeof(Type), componentType)));
                propertyInfo.SetValue(instance, logger, null);
            }));
        }

        private static ILogger CreateLogger(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            var loggerFactory = context.Resolve<ILoggerFactory>();
            var containingType = parameters.TypedAs<Type>();
            return loggerFactory.CreateLogger(containingType);
        }
    }
}