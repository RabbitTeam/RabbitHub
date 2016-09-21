using Autofac;
using FluentMigrator;
using Rabbit.Components.Data.Utility.Extensions;
using Rabbit.Kernel;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.ShellBuilders.Models;
using Rabbit.Kernel.Extensions.Models;
using System;
using System.Linq;

namespace Rabbit.Components.Data.Migrators
{
    /// <summary>
    /// 一个抽象的迁移提供程序。
    /// </summary>
    public interface IMigration : ISingletonDependency
    {
    }

    /// <summary>
    /// 数据迁移基类。
    /// </summary>
    public abstract class MigrationBase : Migration, IMigration
    {
        #region Property

        /// <summary>
        /// 生命周期。
        /// </summary>
        public ILifetimeScope LifetimeScope { get; set; }

        /// <summary>
        /// 功能模型。
        /// </summary>
        public Feature Feature { get; set; }

        private ShellSettings ShellSettings => LifetimeScope.Resolve<ShellSettings>();

        #endregion Property

        #region Protected Method

        /// <summary>
        /// 根据记录类型获取记录对应的表名称。
        /// </summary>
        /// <typeparam name="T">记录类型。</typeparam>
        /// <returns>表名称。</returns>
        protected string TableName<T>() where T : class
        {
            return TableName(typeof(T));
        }

        /// <summary>
        /// 根据记录名称获取对应的表名称。
        /// </summary>
        /// <param name="recordName">记录名称。</param>
        /// <returns>表名称。</returns>
        protected string TableName(string recordName)
        {
            var extensionDescriptor = Feature.Descriptor.Extension;
            var extensionName = extensionDescriptor.Id.Replace('.', '_');

            var dataTablePrefix = string.Empty;
            if (!string.IsNullOrEmpty(ShellSettings.GetDataTablePrefix()))
                dataTablePrefix = ShellSettings.GetDataTablePrefix() + "_";

            return dataTablePrefix + extensionName + '_' + recordName;
        }

        /// <summary>
        /// 获取一个租户内唯一名称（防止外键、主键名称冲突的问题）。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>组合过后的唯一名称。</returns>
        protected string GetOnlyName(string name)
        {
            var prefix = ShellSettings.GetDataTablePrefix();
            return string.IsNullOrEmpty(prefix) ? name : $"{prefix}_{name}";
        }

        #endregion Protected Method

        #region Private Method

        private string TableName(Type recordType)
        {
            var recordBlueprints = LifetimeScope.Resolve<ShellBlueprint>().GetRecords();
            var record = recordBlueprints.First(i => i.Type == recordType);
            return record.TableName;
        }

        #endregion Private Method
    }
}