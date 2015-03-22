using System;

namespace Rabbit.Components.Data.DataAnnotations
{
    /// <summary>
    /// 指定如何将从数据库生成的值的属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DatabaseGeneratedAttribute : Attribute
    {
        /// <summary>
        /// 生成模式。
        /// </summary>
        public DatabaseGeneratedOption DatabaseGeneratedOption
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化一个新的数据库生成标记。
        /// </summary>
        /// <param name="databaseGeneratedOption">数据库生成选项。</param>
        public DatabaseGeneratedAttribute(DatabaseGeneratedOption databaseGeneratedOption)
        {
            if (!Enum.IsDefined(typeof(DatabaseGeneratedOption), databaseGeneratedOption))
            {
                throw new ArgumentOutOfRangeException("databaseGeneratedOption");
            }
            DatabaseGeneratedOption = databaseGeneratedOption;
        }
    }

    /// <summary>
    /// 数据库生成选项。
    /// </summary>
    public enum DatabaseGeneratedOption
    {
        /// <summary>
        /// 不从数据库生成值。
        /// </summary>
        None,

        /// <summary>
        /// 该数据库生成插入一行时的值。
        /// </summary>
        Identity,

        /// <summary>
        /// 数据库时产生一个行被插入或更新的值。
        /// </summary>
        Computed
    }
}