using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rabbit.Kernel.Environment.Assemblies.Models
{
    /// <summary>
    /// 程序集描述符。
    /// </summary>
    public sealed class AssemblyDescriptor
    {
        /// <summary>
        /// 初始化一个新的程序集描述符实例。
        /// </summary>
        public AssemblyDescriptor()
        {
        }

        /// <summary>
        /// 根据程序集名称初始化一个新的程序集描述符实例。
        /// </summary>
        /// <param name="assemblyName">程序集名称。</param>
        /// <exception cref="ArgumentNullException"><paramref name="assemblyName"/> 为空。</exception>
        public AssemblyDescriptor(string assemblyName)
        {
            assemblyName.NotEmptyOrWhiteSpace("assemblyName");

            var fields = assemblyName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var field in fields.Where(i => !string.IsNullOrWhiteSpace(i)))
            {
                var temp = field.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (!temp.Any())
                    continue;

                var key = temp.Length == 1 ? null : temp[0].ToLower();
                var value = temp.Last();

                switch (key)
                {
                    case null:
                        Name = value;
                        break;

                    case "version":
                        Version version;
                        if (Version.TryParse(value, out version))
                            Version = version;
                        break;

                    case "culture":
                        CultureName = value;
                        break;

                    case "publickeytoken":
                        PublicKeyToken = value;
                        break;
                }
            }
        }

        /// <summary>
        /// 初始化一个新的程序集描述符实例。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        public AssemblyDescriptor(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var name = assembly.GetName();
            Name = name.Name;
            Version = name.Version;
            PublicKeyToken = Encoding.UTF8.GetString(name.GetPublicKeyToken());
            CultureName = name.CultureInfo.Name;
        }

        /// <summary>
        /// 程序集名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 程序集版本。
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// 公钥象征。
        /// </summary>
        public string PublicKeyToken { get; set; }

        /// <summary>
        /// 文化名称。
        /// </summary>
        public string CultureName { get; set; }

        /// <summary>
        /// 程序集完全限定名。
        /// </summary>
        public string FullName
        {
            get
            {
                return string.Format("{0}{1}, Culture={2}, PublicKeyToken={3}", Name, Version == null ? string.Empty : string.Format(", Version={0} ", Version), string.IsNullOrWhiteSpace(CultureName) ? "neutral" : CultureName, string.IsNullOrWhiteSpace(PublicKeyToken) ? "null" : PublicKeyToken);
            }
        }

        #region Overrides of Object

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            return FullName;
        }

        #endregion Overrides of Object
    }
}