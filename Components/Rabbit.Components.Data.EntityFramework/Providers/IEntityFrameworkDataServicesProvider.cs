using Rabbit.Components.Data.Providers;
using System.Data.Entity.Core.Common;

namespace Rabbit.Components.Data.EntityFramework.Providers
{
    internal interface IEntityFrameworkDataServicesProvider : IDataServicesProvider
    {
        DbProviderServices Instance { get; }
    }
}