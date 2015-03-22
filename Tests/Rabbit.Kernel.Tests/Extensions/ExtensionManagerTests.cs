using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.Extensions;
using System;
using System.Linq;

namespace Rabbit.Kernel.Tests.Extensions
{
    [TestClass]
    public sealed class ExtensionManagerTests : TestBase
    {
        #region Property

        public IExtensionManager ExtensionManager { get; set; }

        public IExtensionLoaderCoordinator ExtensionLoaderCoordinator { get; set; }

        #endregion Property

        #region Test Method

        [TestMethod]
        public void AvailableExtensionsTest()
        {
            var extensions = ExtensionManager.AvailableExtensions();
            Assert.AreEqual(1, extensions.Count());
        }

        [TestMethod]
        public void AvailableFeaturesTest()
        {
            var features = ExtensionManager.AvailableFeatures();
            Assert.AreEqual(1, features.Count());
        }

        [TestMethod]
        public void GetExtensionTest()
        {
            Assert.IsNotNull(ExtensionManager.GetExtension("Rabbit.Test"));
            Assert.IsNull(ExtensionManager.GetExtension(Guid.NewGuid().ToString()));
        }

        [TestMethod]
        public void LoadFeaturesTest()
        {
            ExtensionLoaderCoordinator.SetupExtensions();
            var features = ExtensionManager.LoadFeatures(ExtensionManager.AvailableFeatures()).ToArray();
            Assert.IsTrue(features.Any());
            Assert.IsTrue(features.SelectMany(i => i.ExportedTypes).Any());
        }

        #endregion Test Method

        #region Overrides of TestBase

        protected override void Register(ContainerBuilder builder)
        {
            builder.RegisterType<TempExtensionDescriptorFilter>().AsImplementedInterfaces();
            builder.RegisterType<TempFeatureDescriptorFilter>().AsImplementedInterfaces();
        }

        #endregion Overrides of TestBase

        #region Help Class

        internal sealed class TempExtensionDescriptorFilter : IExtensionDescriptorFilter
        {
            #region Implementation of IExtensionDescriptorFilter

            /// <summary>
            /// 在发现扩展时执行。
            /// </summary>
            /// <param name="context">扩展描述符条目过滤器上下文。</param>
            public void OnDiscovery(ExtensionDescriptorEntryFilterContext context)
            {
                context.Valid = context.Entry.Id == "Rabbit.Test";
            }

            #endregion Implementation of IExtensionDescriptorFilter
        }

        internal sealed class TempFeatureDescriptorFilter : IFeatureDescriptorFilter
        {
            #region Implementation of IFeatureDescriptorFilter

            /// <summary>
            /// 在发现特性时执行。
            /// </summary>
            /// <param name="context">特性描述符过滤器上下文。</param>
            public void OnDiscovery(FeatureDescriptorFilterContext context)
            {
                context.Valid = context.Feature.Id == context.Feature.Extension.Id;
            }

            #endregion Implementation of IFeatureDescriptorFilter
        }

        #endregion Help Class
    }
}