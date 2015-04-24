using Rabbit.Components.Data.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;

namespace Rabbit.Components.Data.EntityFramework.Conventions
{
    internal sealed class ForeignKeyPrimitivePropertyAttributeConvention : PropertyAttributeConfigurationConvention<ForeignKeyAttribute>
    {
        #region Overrides of PropertyAttributeConfigurationConvention<ForeignKeyAttribute>

        public override void Apply(PropertyInfo memberInfo, ConventionTypeConfiguration configuration, ForeignKeyAttribute attribute)
        {
            new System.Data.Entity.ModelConfiguration.Conventions.ForeignKeyPrimitivePropertyAttributeConvention().Apply(memberInfo, configuration, new System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute(attribute.Name));
        }

        #endregion Overrides of PropertyAttributeConfigurationConvention<ForeignKeyAttribute>
    }
}