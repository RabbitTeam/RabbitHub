using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Components.Logging.NLog;
using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Caching.Impl;
using Rabbit.Kernel.Logging;

namespace Rabbit.Kernel.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        protected IContainer Container { get; private set; }

        protected TestBase()
        {
            var kernelBuilder = new KernelBuilder();
            kernelBuilder.OnStarting(Register);
            kernelBuilder.UseCaching(c => c.UseMemoryCache());
            kernelBuilder.UseLogging(c => c.UseNLog());

            var container = Container = kernelBuilder.Build();
            var type = GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (!container.IsRegistered(propertyType))
                    continue;
                property.SetValue(this, container.Resolve(propertyType), null);
            }
        }

        protected virtual void Register(ContainerBuilder builder)
        {
        }
    }
}