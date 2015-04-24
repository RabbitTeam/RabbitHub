using Rabbit.Components.Data.DataAnnotations;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;

namespace Rabbit.Components.Data.EntityFramework.Conventions
{
    internal sealed class NotMappedPropertyAttributeConvention : PropertyAttributeConfigurationConvention<NotMappedAttribute>
    {
        #region Overrides of PropertyAttributeConfigurationConvention<NotMappedAttribute>

        public override void Apply(PropertyInfo memberInfo, ConventionTypeConfiguration configuration, NotMappedAttribute attribute)
        {
            if (memberInfo == null)
                throw new ArgumentNullException("memberInfo");
            if (attribute == null)
                throw new ArgumentNullException("attribute");
            if (attribute == null)
                throw new ArgumentNullException("attribute");
            configuration.Ignore(memberInfo);
        }

        #endregion Overrides of PropertyAttributeConfigurationConvention<NotMappedAttribute>
    }
}