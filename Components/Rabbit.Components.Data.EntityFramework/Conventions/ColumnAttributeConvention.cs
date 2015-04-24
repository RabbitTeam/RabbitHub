using Rabbit.Components.Data.DataAnnotations;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Rabbit.Components.Data.EntityFramework.Conventions
{
    internal sealed class ColumnAttributeConvention : PrimitivePropertyAttributeConfigurationConvention<ColumnAttribute>
    {
        #region Overrides of PrimitivePropertyAttributeConfigurationConvention<ColumnAttribute>

        public override void Apply(ConventionPrimitivePropertyConfiguration configuration, ColumnAttribute attribute)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            if (!string.IsNullOrWhiteSpace(attribute.Name))
            {
                configuration.HasColumnName(attribute.Name);
            }
            if (!string.IsNullOrWhiteSpace(attribute.TypeName))
            {
                configuration.HasColumnType(attribute.TypeName);
            }
            if (attribute.Order >= 0)
            {
                configuration.HasColumnOrder(attribute.Order);
            }
        }

        #endregion Overrides of PrimitivePropertyAttributeConfigurationConvention<ColumnAttribute>
    }
}