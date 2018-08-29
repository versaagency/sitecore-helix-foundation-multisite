using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Foundation.Multisite.Placeholders;

namespace Sitecore.Foundation.Multisite
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.Replace(ServiceDescriptor.Singleton<BasePlaceholderCacheManager, SiteSpecificPlaceholderCacheManager>());
        }
    }
}
