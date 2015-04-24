using Rabbit.Kernel;
using System;
using System.Data.Entity.ModelConfiguration;

namespace Rabbit.Components.Data.EntityFramework
{
    /// <summary>
    /// 一个抽象的映射接口。
    /// </summary>
    public interface IMapping : IDependency
    {
        /// <summary>
        /// 实体类型。
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// 映射动作。
        /// </summary>
        /// <param name="configuration">配置对象。</param>
        void Mapping(object configuration);
    }

    /// <summary>
    /// 映射抽象类。
    /// </summary>
    /// <typeparam name="TEntityType">实体类型。</typeparam>
    public abstract class MappingBase<TEntityType> : IMapping where TEntityType : class
    {
        /// <summary>
        /// 映射动作。
        /// </summary>
        /// <param name="configuration">实体类型配置对象。</param>
        public abstract void Mapping(EntityTypeConfiguration<TEntityType> configuration);

        #region Implementation of IMapping

        /// <summary>
        /// 实体类型。
        /// </summary>
        public Type Type { get { return typeof(TEntityType); } }

        /// <summary>
        /// 映射动作。
        /// </summary>
        /// <param name="configuration">实体类型配置对象。</param>
        public void Mapping(object configuration)
        {
            Mapping(configuration as EntityTypeConfiguration<TEntityType>);
        }

        #endregion Implementation of IMapping
    }
}