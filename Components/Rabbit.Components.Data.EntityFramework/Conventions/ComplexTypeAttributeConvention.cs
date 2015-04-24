using Rabbit.Components.Data.DataAnnotations;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Rabbit.Components.Data.EntityFramework.Conventions
{
    internal sealed class ComplexTypeAttributeConvention : TypeAttributeConfigurationConvention<ComplexTypeAttribute>
    {
        #region Overrides of TypeAttributeConfigurationConvention<ComplexTypeAttribute>

        public override void Apply(ConventionTypeConfiguration configuration, ComplexTypeAttribute attribute)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            configuration.IsComplexType();
        }

        #endregion Overrides of TypeAttributeConfigurationConvention<ComplexTypeAttribute>
    }
}