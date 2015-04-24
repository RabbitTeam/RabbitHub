using System;

namespace Rabbit.Components.Web.SignalR
{
    /// <summary>
    /// 允许自定义连接名称和URL。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConnectionAttribute : Attribute
    {
        #region Constructor

        public ConnectionAttribute(string name)
            : this(name, string.Empty)
        {
        }

        public ConnectionAttribute(string name, string url)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("连接名称不能为空。", "name");

            Name = name;
            Url = url;
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 连接名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 连接URL。
        /// </summary>
        public string Url { get; set; }

        #endregion Property
    }
}