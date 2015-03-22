using Autofac;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Extensions.Folders;
using Rabbit.Kernel.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rabbit.Kernel
{
    /// <summary>
    ///     一个抽象的内核建设者。
    /// </summary>
    public interface IKernelBuilder
    {
        /// <summary>
        ///     在启动前执行。
        /// </summary>
        /// <param name="action">动作。</param>
        /// <param name="actionName">动作名称，如果动作名称不为空并且存在相同的动作名称则用最后一个动作替换之前的动作。</param>
        /// <returns>内核建设者。</returns>
        IKernelBuilder OnStarting(Action<ContainerBuilder> action, string actionName = null);

        /// <summary>
        ///     在启动完成之后执行。
        /// </summary>
        /// <param name="action">动作。</param>
        /// <param name="actionName">动作名称，如果动作名称不为空并且存在相同的动作名称则用最后一个动作替换之前的动作。</param>
        /// <returns>内核建设者。</returns>
        IKernelBuilder OnStarted(Action<IContainer> action, string actionName = null);

        /// <summary>
        ///     建造。
        /// </summary>
        /// <returns>容器。</returns>
        IContainer Build();
    }

    /// <summary>
    ///     内核建设者。
    /// </summary>
    public sealed class KernelBuilder : IKernelBuilder
    {
        #region Field

        private readonly IDictionary<string, Delegate> _actionDictionary = new Dictionary<string, Delegate>();
        private int _actionIdentity;

        #endregion Field

        #region Constructor

        /// <summary>
        ///     初始化一个新的内核建设者。
        /// </summary>
        public KernelBuilder()
        {
            OnStarted(container =>
            {
                var hostContainer = container.Resolve<IHostContainer>(new TypedParameter(typeof(IContainer), container));
                HostContainerRegistry.RegisterHostContainer(hostContainer);
            });
        }

        #endregion Constructor

        #region Implementation of IKernelBuilder

        /// <summary>
        ///     在启动前执行。
        /// </summary>
        /// <param name="action">动作。</param>
        /// <param name="actionName">动作名称，如果动作名称不为空并且存在相同的动作名称则用最后一个动作替换之前的动作。</param>
        /// <returns>内核建设者。</returns>
        public IKernelBuilder OnStarting(Action<ContainerBuilder> action, string actionName = null)
        {
            var name = GetActionName(actionName);
            _actionDictionary[name] = action;

            return this;
        }

        /// <summary>
        ///     在启动完成之后执行。
        /// </summary>
        /// <param name="action">动作。</param>
        /// <param name="actionName">动作名称，如果动作名称不为空并且存在相同的动作名称则用最后一个动作替换之前的动作。</param>
        /// <returns>内核建设者。</returns>
        public IKernelBuilder OnStarted(Action<IContainer> action, string actionName = null)
        {
            var name = GetActionName(actionName);
            _actionDictionary[name] = action;

            return this;
        }

        /// <summary>
        ///     建造。
        /// </summary>
        /// <returns>容器。</returns>
        public IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<KernelModule>();

            foreach (var action in GetBuildingActions())
                action(builder);

            var container = builder.Build();

            foreach (var action in GetBuildedActions())
                action(container);

            return container;
        }

        #endregion Implementation of IKernelBuilder

        #region Private Method

        private IEnumerable<Action<ContainerBuilder>> GetBuildingActions()
        {
            return
                _actionDictionary.Where(i => i.Value is Action<ContainerBuilder>)
                    .Select(i => i.Value)
                    .OfType<Action<ContainerBuilder>>()
                    .ToArray();
        }

        private IEnumerable<Action<IContainer>> GetBuildedActions()
        {
            return
                _actionDictionary.Where(i => i.Value is Action<IContainer>)
                    .Select(i => i.Value)
                    .OfType<Action<IContainer>>()
                    .ToArray();
        }

        private string GetActionName(string actionName)
        {
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                return "Custom_" + actionName;
            }
            _actionIdentity = _actionIdentity + 1;
            return "Default_" + _actionIdentity;
        }

        #endregion Private Method
    }

    /// <summary>
    ///     内核建设者扩展方法。
    /// </summary>
    public static class KernelBuilderExtensions
    {
        #region Field

        private static readonly List<ExtensionFolders.SimpleExtensionDescription> Descriptions =
            new List<ExtensionFolders.SimpleExtensionDescription>();

        #endregion Field

        /// <summary>
        ///     注册一个扩展。
        /// </summary>
        /// <param name="kernelBuilder">内核建设者。</param>
        /// <param name="assembly">扩展程序集。</param>
        /// <returns>内核建设者。</returns>
        public static IKernelBuilder RegisterExtension(this IKernelBuilder kernelBuilder, Assembly assembly)
        {
            return RegisterExtension(kernelBuilder, assembly, true);
        }

        /// <summary>
        ///     注册一个扩展。
        /// </summary>
        /// <param name="kernelBuilder">内核建设者。</param>
        /// <param name="assembly">扩展程序集。</param>
        /// <param name="isMinimumShellDescriptor">是否迷你外壳描述符。</param>
        /// <returns>内核建设者。</returns>
        public static IKernelBuilder RegisterExtension(this IKernelBuilder kernelBuilder, Assembly assembly, bool isMinimumShellDescriptor)
        {
            kernelBuilder.OnStarting(
                builder =>
                {
                    builder.RegisterType<ExtensionFolders>().As<IExtensionFolders>().SingleInstance();
                    builder.RegisterType<ExtensionMinimumShellDescriptorProvider>().As<IMinimumShellDescriptorProvider>().SingleInstance();
                },
                "RegisterExtensions");

            var description = new ExtensionFolders.SimpleExtensionDescription
            {
                Id = assembly.GetName().Name,
                Type = "Module",
                Location = "~/"
            };
            Descriptions.Add(description);

            if (isMinimumShellDescriptor)
            {
                ExtensionMinimumShellDescriptorProvider.Features.Add(description.Id);
            }

            return kernelBuilder;
        }

        internal sealed class ExtensionFolders : IExtensionFolders
        {
            #region Implementation of IExtensionFolders

            /// <summary>
            ///     可用的扩展。
            /// </summary>
            /// <returns>扩展描述条目符集合。</returns>
            public IEnumerable<ExtensionDescriptorEntry> AvailableExtensions()
            {
                var entrys =
                    Descriptions.Select(i => GetExtensionDescriptorEntry(i.Id, i.Type, i.Location));
                return entrys;
            }

            #endregion Implementation of IExtensionFolders

            #region Private Method

            private static ExtensionDescriptorEntry GetExtensionDescriptorEntry(string id, string type, string localtion)
            {
                var extension = new ExtensionDescriptorEntry(new ExtensionDescriptor(), id, type, localtion);
                extension.Descriptor.Features = new[]
                {
                    new FeatureDescriptor
                    {
                        Id = extension.Id,
                        Extension = extension
                    }
                };
                return extension;
            }

            #endregion Private Method

            #region Help Class

            public class SimpleExtensionDescription
            {
                public string Id { get; set; }

                public string Type { get; set; }

                public string Location { get; set; }
            }

            #endregion Help Class
        }
    }

    internal sealed class ExtensionMinimumShellDescriptorProvider : IMinimumShellDescriptorProvider
    {
        public static readonly List<string> Features = new List<string>();

        #region Implementation of IMinimumShellDescriptorProvider

        /// <summary>
        /// 获取外壳描述符。
        /// </summary>
        /// <param name="features">外壳特性描述符。</param>
        public void GetFeatures(ICollection<ShellFeature> features)
        {
            foreach (var feature in Features.Distinct().Select(i => new ShellFeature { Name = i }))
            {
                features.Add(feature);
            }
        }

        #endregion Implementation of IMinimumShellDescriptorProvider
    }
}