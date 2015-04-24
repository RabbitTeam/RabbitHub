using Microsoft.AspNet.SignalR;
using Rabbit.Kernel;

namespace Rabbit.Components.Web.SignalR
{
    internal interface IRabbitHubConfiguration : IDependency
    {
        HubConfiguration ConnectionConfiguration { get; }
    }
}