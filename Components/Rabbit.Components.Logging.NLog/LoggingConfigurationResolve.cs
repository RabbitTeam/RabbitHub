using NLog.Config;
using Rabbit.Kernel.Caching;
using Rabbit.Kernel.FileSystems.VirtualPath;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Rabbit.Components.Logging.NLog
{
    /// <summary>
    /// 日志配置文件解析接口。
    /// <remarks>全局单例。</remarks>
    /// </summary>
    internal interface ILoggingConfigurationResolve
    {
        /// <summary>
        /// 根据程序集获取日志记录器配置信息。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        /// <returns>日志配置。</returns>
        object GetLoggingConfiguration(Assembly assembly);
    }

    internal sealed class LoggingConfigurationResolve : ILoggingConfigurationResolve
    {
        #region Field

        private readonly string[] _configNames = { "{0}.LoggerConfig.config", "{0}.Config.Log.LoggerConfig.config", "{0}.Configs.Log.LoggerConfig.config" };
        private readonly string _defaultConfigurationPath;
        private readonly XDocument _defaultLoggingConfigurationDocument;
        private readonly ICacheManager _cacheManager;

        #endregion Field

        #region Constructor

        public LoggingConfigurationResolve(ICacheManager cacheManager, IVirtualPathProvider virtualPathProvider)
        {
            _defaultConfigurationPath = virtualPathProvider.MapPath(@"~/Config/Logging/LoggingConfig.config");
            _defaultLoggingConfigurationDocument = XDocument.Load(_defaultConfigurationPath);
            _cacheManager = cacheManager;
        }

        #endregion Constructor

        #region Implementation of ILoggingConfigurationResolve

        /// <summary>
        /// 根据程序集获取日志记录器配置信息。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        /// <returns>日志配置。</returns>
        public object GetLoggingConfiguration(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return _cacheManager.Get(assembly.GetHashCode(), ctx =>
            {
                var stream = GetLoggingConfigurationStream(assembly);

                if (stream == null)
                    return new XmlLoggingConfiguration(_defaultConfigurationPath);

                using (stream)
                {
                    var document = XDocument.Load(stream);

                    var nlogElement = document.Element(XName.Get("nlog", "http://www.nlog-project.org/schemas/NLog.xsd"));

                    //组合规则。
                    CombineRules(assembly, document);

                    //组合目标。
                    CombineTargets(nlogElement);
                    using (var stringReader = new StringReader(document.ToString()))
                    {
                        var xmlReader = XmlReader.Create(stringReader);
                        return new XmlLoggingConfiguration(xmlReader, null);
                    }
                }
            });
        }

        #endregion Implementation of ILoggingConfigurationResolve

        #region Private Method

        /// <summary>
        /// 根据程序集获取日志记录器配置文件流。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        /// <returns>文件流。</returns>
        private Stream GetLoggingConfigurationStream(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var assemblyName = assembly.GetName().Name;
            var fileName = assembly.GetManifestResourceNames().SingleOrDefault(rName => _configNames.Any(name => string.Equals(string.Format(name, assemblyName), rName, StringComparison.OrdinalIgnoreCase)));
            return string.IsNullOrWhiteSpace(fileName) ? null : assembly.GetManifestResourceStream(fileName);
        }

        /// <summary>
        /// 组合规则。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        /// <param name="container">Xml配置文档。</param>
        private static void CombineRules(Assembly assembly, XContainer container)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var assemblyName = assembly.GetName().Name;

            var nameAttributes = container.Descendants()
                    .Where(
                        xElement => string.Equals(xElement.Name.LocalName, "logger", StringComparison.OrdinalIgnoreCase)).Select(xElement => xElement.Attribute("name"));

            foreach (var nameAttribute in nameAttributes)
            {
                nameAttribute.SetValue(string.Format("{0}.{1}", assemblyName, nameAttribute.Value));
            }
        }

        /// <summary>
        /// 组合目标。
        /// </summary>
        /// <param name="container">需要组合的容器。</param>
        private void CombineTargets(XContainer container)
        {
            var targetElements = _defaultLoggingConfigurationDocument.Descendants().Where(xElement => string.Equals(xElement.Name.LocalName, "target", StringComparison.OrdinalIgnoreCase));

            var targetsElement = new XElement("targets");

            foreach (var targetElement in targetElements)
            {
                targetElement.Name = XName.Get("target");
                targetsElement.Add(targetElement);
            }

            container.AddFirst(targetsElement);
        }

        #endregion Private Method
    }
}