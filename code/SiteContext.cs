using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Foundation.DependencyInjection;
using Sitecore.Foundation.Multisite.Providers;

namespace Sitecore.Foundation.Multisite
{
    [Service]
    public class SiteContext
    {
        private readonly ISiteDefinitionsProvider siteDefinitionsProvider;

        public SiteContext(ISiteDefinitionsProvider siteDefinitionsProvider)
        {
            this.siteDefinitionsProvider = siteDefinitionsProvider;
        }

        public virtual SiteDefinition GetSiteDefinition([NotNull] Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));

            return this.siteDefinitionsProvider.GetContextSiteDefinition(item);
        }
    }
}
