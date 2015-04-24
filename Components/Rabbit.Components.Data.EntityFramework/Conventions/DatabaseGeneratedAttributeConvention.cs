using Rabbit.Components.Data.DataAnnotations;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Rabbit.Components.Data.EntityFramework.Conventions
{
    internal sealed class DatabaseGeneratedAttributeConvention : PrimitivePropertyAttributeConfigurationConvention<DatabaseGeneratedAttribute>
    {
        #region Overrides of PrimitivePropertyAttributeConfigurationConvention<DatabaseGeneratedAttribute>

        public override void Apply(ConventionPrimitivePropertyConfiguration configuration, DatabaseGeneratedAttribute attribute)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption option;
            switch (attribute.DatabaseGeneratedOption)
            {
                case DatabaseGeneratedOption.Identity:
                    option = System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity;
                    break;

                case DatabaseGeneratedOption.Computed:
                    option = System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed;
                    break;

                default:
                    option = System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None;
                    break;
            }

            configuration.HasDatabaseGeneratedOption(option);
        }

        #endregion Overrides of PrimitivePropertyAttributeConfigurationConvention<DatabaseGeneratedAttribute>
    }
}