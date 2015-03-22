using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.FileSystems.AppData;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Rabbit.Kernel.Environment.Descriptor.Impl
{
    internal sealed class DefaultShellDescriptorCache : IShellDescriptorCache
    {
        #region Field

        private readonly IAppDataFolder _appDataFolder;
        private const string DescriptorCacheFileName = "cache.dat";
        private static readonly object SynLock = new object();

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        public Localizer T { get; set; }

        public bool Disabled { get; set; }

        #endregion Property

        #region Constructor

        public DefaultShellDescriptorCache(IAppDataFolder appDataFolder)
        {
            _appDataFolder = appDataFolder;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IShellDescriptorCache

        /// <summary>
        /// 从缓存中抓取外壳描述符。
        /// </summary>
        /// <param name="shellName">外壳名称。</param>
        /// <returns>外壳描述符。</returns>
        public ShellDescriptor Fetch(string shellName)
        {
            if (Disabled)
                return null;

            lock (SynLock)
            {
                if (!VerifyCacheFile())
                    return null;

                var text = _appDataFolder.ReadFile(DescriptorCacheFileName);
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                XmlNode rootNode = xmlDocument.DocumentElement;
                if (rootNode == null)
                    return null;
                foreach (XmlNode tenantNode in rootNode.ChildNodes)
                {
                    if (string.Equals(tenantNode.Name, shellName, StringComparison.OrdinalIgnoreCase))
                    {
                        return GetShellDecriptorForCacheText(tenantNode.InnerText);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 将一个外壳描述符存储到缓存中。
        /// </summary>
        /// <param name="shellName">外壳名称。</param>
        /// <param name="descriptor">外壳描述符。</param>
        public void Store(string shellName, ShellDescriptor descriptor)
        {
            if (Disabled)
                return;

            lock (SynLock)
            {
                VerifyCacheFile();
                var text = _appDataFolder.ReadFile(DescriptorCacheFileName);
                var tenantCacheUpdated = false;
                var saveWriter = new StringWriter();
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                XmlNode rootNode = xmlDocument.DocumentElement;
                if (rootNode != null)
                {
                    foreach (var tenantNode in rootNode.ChildNodes.Cast<XmlNode>().Where(tenantNode => string.Equals(tenantNode.Name, shellName, StringComparison.OrdinalIgnoreCase)))
                    {
                        tenantNode.InnerText = GetCacheTextForShellDescriptor(descriptor);
                        tenantCacheUpdated = true;
                        break;
                    }
                    if (!tenantCacheUpdated)
                    {
                        var newTenant = xmlDocument.CreateElement(shellName);
                        newTenant.InnerText = GetCacheTextForShellDescriptor(descriptor);
                        rootNode.AppendChild(newTenant);
                    }
                }

                xmlDocument.Save(saveWriter);
                _appDataFolder.CreateFile(DescriptorCacheFileName, saveWriter.ToString());
            }
        }

        #endregion Implementation of IShellDescriptorCache

        #region Private Method

        private static string GetCacheTextForShellDescriptor(ShellDescriptor descriptor)
        {
            var sb = new StringBuilder();
            sb.Append(descriptor.SerialNumber + "|");
            foreach (var feature in descriptor.Features)
            {
                sb.Append(feature.Name + ";");
            }

            return sb.ToString();
        }

        private static ShellDescriptor GetShellDecriptorForCacheText(string p)
        {
            var fields = p.Trim().Split(new[] { "|" }, StringSplitOptions.None);
            var shellDescriptor = new ShellDescriptor { SerialNumber = Convert.ToInt32(fields[0]) };
            var features = fields[1].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            shellDescriptor.Features = features.Select(feature => new ShellFeature { Name = feature }).ToList();

            return shellDescriptor;
        }

        private bool VerifyCacheFile()
        {
            if (_appDataFolder.FileExists(DescriptorCacheFileName))
                return true;

            var writer = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(writer))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Tenants");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            _appDataFolder.CreateFile(DescriptorCacheFileName, writer.ToString());

            return false;
        }

        #endregion Private Method
    }
}