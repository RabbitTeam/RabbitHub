using Rabbit.Components.Data.DataAnnotations;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Rabbit.Components.Data.EntityFramework.Conventions
{
    internal sealed class NotMappedTypeAttributeConvention : TypeAttributeConfigurationConvention<NotMappedAttribute>
    {
        #region Overrides of TypeAttributeConfigurationConvention<NotMappedAttribute>

        public override void Apply(ConventionTypeConfiguration configuration, NotMappedAttribute attribute)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            if (attribute == null)
                throw new ArgumentNullException("attribute");
            configuration.Ignore();
        }

        #endregion Overrides of TypeAttributeConfigurationConvention<NotMappedAttribute>
    }
}