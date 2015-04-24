using Rabbit.Components.Data.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;

namespace Rabbit.Components.Data.EntityFramework.Conventions
{
    internal sealed class InversePropertyAttributeConvention : PropertyAttributeConfigurationConvention<InversePropertyAttribute>
    {
        #region Overrides of PropertyAttributeConfigurationConvention<InversePropertyAttribute>

        public override void Apply(PropertyInfo memberInfo, ConventionTypeConfiguration configuration, InversePropertyAttribute attribute)
        {
            new System.Data.Entity.ModelConfiguration.Conventions.InversePropertyAttributeConvention().Apply(memberInfo, configuration, new System.ComponentModel.DataAnnotations.Schema.InversePropertyAttribute(attribute.Property));
        }

        #endregion Overrides of PropertyAttributeConfigurationConvention<InversePropertyAttribute>
    }
}