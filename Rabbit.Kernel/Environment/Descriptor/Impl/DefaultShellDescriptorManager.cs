using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.FileSystems.Application;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Rabbit.Kernel.Environment.Descriptor.Impl
{
    internal sealed class DefaultShellDescriptorManager : IShellDescriptorManager
    {
        #region Field

        private readonly IExtensionManager _extensionManager;
        private readonly IEnumerable<IShellDescriptorManagerEventHandler> _events;
        private readonly ShellSettings _settings;

        private static readonly PersistenceList<ShellDescriptorEntry> List = new PersistenceList<ShellDescriptorEntry>();
        private static readonly object SyncLock = new object();

        private static int _serialNumber;

        #endregion Field

        #region Property

        public Localizer T { get; set; }

        #endregion Property

        #region Constructor

        public DefaultShellDescriptorManager(IExtensionManager extensionManager, IEnumerable<IShellDescriptorManagerEventHandler> events, ShellSettings settings, IApplicationFolder applicationFolder)
        {
            _extensionManager = extensionManager;
            _events = events;
            _settings = settings;

            List.Init("~/App_Data/descriptors.dat", applicationFolder);

            var descriptor = List.OrderByDescending(i => i.SerialNumber).FirstOrDefault();
            _serialNumber = descriptor == null ? 0 : descriptor.SerialNumber;

            T = NullLocalizer.Instance;
        }

        #endregion Constructor

        #region Implementation of IShellDescriptorManager

        /// <summary>
        /// 获取当前外壳描述符。
        /// </summary>
        /// <returns>外壳描述符。</returns>
        public ShellDescriptor GetShellDescriptor()
        {
            //得到当前租户的外壳描述符。
            var descriptor = GetCurrentShellDescriptor();

            //如果之前的记录与最新的功能描述符相等直接返回。
            if (descriptor != null)
                return (ShellDescriptor)descriptor;
            /*//如果之前的记录与最新的功能描述符相等直接返回。
            if (descriptor != null && features.SequenceEqual(descriptor.Features.Select(i => i.Name)))
                return (ShellDescriptor)descriptor;

            //删除之前的描述符。
            if (descriptor != null)
                DeleteShellDescriptor(descriptor, false);*/

            //得到最新的功能描述符名称集合。
            var features = GetFeatures();
            //添加一个新的描述符。
            descriptor = new ShellDescriptorEntry
            {
                ShellName = _settings.Name,
                Features = features.Select(i => new ShellFeature { Name = i }).Distinct(new ShellFeatureEqualityComparer()).ToArray(),
                SerialNumber = GetSerialNumber()
            };

            List.Add(descriptor);
            List.Save();

            return (ShellDescriptor)descriptor;
        }

        /// <summary>
        /// 更新外壳描述符。
        /// </summary>
        /// <param name="serialNumber">序列号。</param>
        /// <param name="enabledFeatures">需要开启的特性。</param>
        public void UpdateShellDescriptor(int serialNumber, IEnumerable<ShellFeature> enabledFeatures)
        {
            if (enabledFeatures == null)
                DeleteShellDescriptor();
            else
            {
                var features = enabledFeatures.Distinct(new ShellFeatureEqualityComparer()).ToArray();
                var descriptor = new ShellDescriptor
                {
                    SerialNumber = serialNumber,
                    Features = features
                };
                List.Add(new ShellDescriptorEntry
                {
                    Features = features,
                    SerialNumber = descriptor.SerialNumber,
                    ShellName = _settings.Name
                });
                List.Save();
                foreach (var handler in _events)
                    handler.Changed(descriptor, _settings.Name);
            }
        }

        #endregion Implementation of IShellDescriptorManager

        #region Private Method

        private IEnumerable<string> GetFeatures()
        {
            var features =
                new[] { "Rabbit.Kernel" }.Concat(
                    _extensionManager.AvailableFeatures().Select(i => i.Id)).ToArray();

            return features;
        }

        private void DeleteShellDescriptor()
        {
            var model = List.FirstOrDefault(i => string.Equals(i.ShellName, _settings.Name, StringComparison.OrdinalIgnoreCase));
            if (model == null)
                return;
            List.Remove(model);
        }

        /*        private void UpdateShellDescriptor(ShellDescriptorEntry descriptor, IEnumerable<ShellFeature> enabledFeatures)
                {
                    if (descriptor == null)
                        throw new InvalidOperationException(T("找不到租户 '{0}' 的外壳描述符。", _settings.Name).ToString());

                    if (enabledFeatures != null)
                    {
                        descriptor.Features = enabledFeatures.ToArray();
                    }
                    else
                    {
                        DeleteShellDescriptor(descriptor, false);
                    }

                    List.Save();

                    foreach (var handler in _events)
                        handler.Changed((ShellDescriptor)descriptor, _settings.Name);
                }*/

        private static int GetSerialNumber()
        {
            lock (SyncLock)
            {
                _serialNumber = _serialNumber + 1;
                return _serialNumber;
            }
        }

        private ShellDescriptorEntry GetCurrentShellDescriptor()
        {
            return List.FirstOrDefault(i => _settings.Name.Equals(i.ShellName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Private Method
    }

    /// <summary>
    /// 外壳描述符条目。
    /// </summary>
    public sealed class ShellDescriptorEntry
    {
        /// <summary>
        /// 序列号。
        /// </summary>
        [XmlAttribute]
        public int SerialNumber { get; set; }

        /// <summary>
        /// 外壳名称。
        /// </summary>
        [XmlAttribute]
        public string ShellName { get; set; }

        /// <summary>
        /// 特性集合。
        /// </summary>
        public ShellFeature[] Features { get; set; }

        /// <summary>
        /// 将外壳描述符条目转换成外壳描述符。
        /// </summary>
        /// <param name="entry">外壳描述符条目。</param>
        /// <returns>外壳描述符。</returns>
        public static explicit operator ShellDescriptor(ShellDescriptorEntry entry)
        {
            return new ShellDescriptor
            {
                SerialNumber = entry.SerialNumber,
                Features = entry.Features
            };
        }
    }
}