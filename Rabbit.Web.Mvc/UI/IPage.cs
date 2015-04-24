using Rabbit.Web.Mvc.DisplayManagement.Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.UI
{
    /// <summary>
    /// 一个抽象的页面接口。
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// 区域集合。
        /// </summary>
        IZoneCollection Zones { get; }
    }

    /// <summary>
    /// 区域集合接口。
    /// </summary>
    public interface IZoneCollection
    {
        /// <summary>
        /// 根据 <paramref name="key"/> 获取区域实例，如果对应的区域不存在则创建一个新的区域并返回。
        /// </summary>
        /// <param name="key">区域key。</param>
        /// <returns>区域实例。</returns>
        IZone this[string key] { get; }
    }

    /// <summary>
    /// 区域集合。
    /// </summary>
    internal sealed class ZoneCollection : IZoneCollection
    {
        private readonly IDictionary<string, IZone> _zones = new Dictionary<string, IZone>();

        #region Implementation of IZoneCollection

        /// <summary>
        /// 根据 <paramref name="key"/> 获取区域实例，如果对应的区域不存在则创建一个新的区域并返回。
        /// </summary>
        /// <param name="key">区域key。</param>
        /// <returns>区域实例。</returns>
        public IZone this[string key]
        {
            get
            {
                if (!_zones.ContainsKey(key))
                    _zones[key] = new Zone();
                return _zones[key];
            }
        }

        #endregion Implementation of IZoneCollection
    }

    /// <summary>
    /// 一个抽象的区域接口。
    /// </summary>
    public interface IZone : IEnumerable
    {
        /// <summary>
        /// 区域名称。
        /// </summary>
        string ZoneName { get; set; }

        /// <summary>
        /// 添加形状。
        /// </summary>
        /// <param name="item">形状项。</param>
        /// <param name="position">位置。</param>
        /// <returns>形状。</returns>
        Shape Add(object item, string position);

        /// <summary>
        /// 添加形状。
        /// </summary>
        /// <param name="action">对Html助手的动作。</param>
        /// <param name="position">位置。</param>
        /// <returns>区域本身。</returns>
        IZone Add(Action<HtmlHelper> action, string position);
    }

    /// <summary>
    /// 区域实现。
    /// </summary>
    public sealed class Zone : Shape, IZone
    {
        #region Implementation of IZone

        /// <summary>
        /// 区域名称。
        /// </summary>
        public string ZoneName { get; set; }

        /// <summary>
        /// 添加形状。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public IZone Add(Action<HtmlHelper> action, string position)
        {
            return this;
        }

        #endregion Implementation of IZone
    }
}