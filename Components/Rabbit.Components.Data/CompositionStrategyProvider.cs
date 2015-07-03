using Rabbit.Components.Data.Models;
using Rabbit.Components.Data.Utility.Extensions;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Extensions.Models;
using System;
using System.Linq;

namespace Rabbit.Components.Data
{
    internal sealed class CompositionStrategyProvider : ICompositionStrategyProvider
    {
        #region Implementation of ICompositionStrategyProvider

        /// <summary>
        /// 应用。
        /// </summary>
        /// <param name="context">组合策略应用上下文。</param>
        public void Apply(CompositionStrategyApplyContext context)
        {
            var records = context.BuildBlueprint(IsRecord, (t, f) => BuildRecord(t, f, context.ShellBlueprint.Settings));
            context.ShellBlueprint.SetRecords(records);
        }

        #endregion Implementation of ICompositionStrategyProvider

        #region Private Method

        private static bool IsRecord(Type type)
        {
            return !type.IsAbstract && type.IsClass && (typeof(IEntity).IsAssignableFrom(type) || type.GetCustomAttributes(typeof(EntityAttribute), false).Any());
        }

        private static RecordBlueprint BuildRecord(Type type, Feature feature, ShellSettings settings)
        {
            var extensionDescriptor = feature.Descriptor.Extension;
            var extensionName = extensionDescriptor.Id.Replace('.', '_');

            var dataTablePrefix = string.Empty;
            if (!string.IsNullOrEmpty(settings.GetDataTablePrefix()))
                dataTablePrefix = settings.GetDataTablePrefix() + "_";

            return new RecordBlueprint
            {
                Type = type,
                Feature = feature,
                TableName = dataTablePrefix + extensionName + '_' + type.Name,
            };
        }

        #endregion Private Method
    }
}