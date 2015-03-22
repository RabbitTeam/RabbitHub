using System;
using System.Data;

namespace Rabbit.Components.Data.DataAnnotations
{
    /// <summary>
    /// 列属性标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        private string _typeName;
        private int _order = -1;

        /// <summary>
        /// 列名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 排序。
        /// </summary>
        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                _order = value;
            }
        }

        /// <summary>
        /// 类型名称。
        /// </summary>
        public string TypeName
        {
            get
            {
                return _typeName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("value");
                _typeName = value;
            }
        }

        /// <summary>
        /// 初始化一个新的列标记。
        /// </summary>
        public ColumnAttribute()
        {
        }

        /// <summary>
        /// 初始化一个新的列标记。
        /// </summary>
        /// <param name="name">列名称。</param>
        public ColumnAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            Name = name;
        }

        /// <summary>
        /// 初始化一个新的列标记。
        /// </summary>
        /// <param name="dbType">数据类型。</param>
        public ColumnAttribute(DbType dbType)
        {
            TypeName = dbType.ToString();
        }

        /// <summary>
        /// 初始化一个新的列标记。
        /// </summary>
        /// <param name="name">列名称。</param>
        /// <param name="dbType">数据类型。</param>
        public ColumnAttribute(string name, DbType dbType)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            Name = name;
            _typeName = dbType.ToString();
        }
    }
}