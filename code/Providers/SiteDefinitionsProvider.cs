using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Foundation.DependencyInjection;
using Sitecore.Foundation.SitecoreExtensions.Extensions;
using Sitecore.Web;

namespace Sitecore.Foundation.Multisite.Providers
{
    [Service(typeof(ISiteDefinitionsProvider))]
    public class SiteDefinitionsProvider : ISiteDefinitionsProvider
    {
        private readonly IEnumerable<SiteInfo> sites;
        private IEnumerable<SiteDefinition> siteDefinitions;

        public SiteDefinitionsProvider(BaseSiteContextFactory siteContextFactory)
        {
            this.sites = siteContextFactory.GetSites();
        }

        public IEnumerable<SiteDefinition> SiteDefinitions => this.siteDefinitions ?? (this.siteDefinitions = this.sites.Where(this.IsValidSite).Select(this.Create).OrderBy(s => s.Item.Appearance.Sortorder).ToArray());

        public SiteDefinition GetContextSiteDefinition(Item item)
        {
            return this.GetSiteByHierarchy(item) ?? this.SiteDefinitions.FirstOrDefault(s => s.IsCurrent);
        }

        public SiteDefinition GetSiteDefinition(string name)
            => SiteDefinitions.FirstOrDefault(ent => ent.Site.Name.Equals(name));

        private static string GetHostName(SiteInfo site)
        {
            if (!string.IsNullOrEmpty(site.TargetHostName))
            {
                return site.TargetHostName;
            }

            if (Uri.CheckHostName(site.HostName) != UriHostNameType.Unknown)
            {
                return site.HostName;
            }

            throw new ConfigurationErrorsException($"Cannot determine hostname for site '{site}'");
        }

        private static bool IsSite([NotNull] Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return item.IsDerived(Templates.Site.ID);
        }

        private bool IsValidSite([NotNull] SiteInfo site)
        {
            return this.GetSiteRootItem(site) != null;
        }

        private Item GetSiteRootItem(SiteInfo site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            if (string.IsNullOrEmpty(site.Database))
            {
                return null;
            }

            var database = Database.GetDatabase(site.Database);
            var item = database?.GetItem(site.RootPath);
            if (item == null || !IsSite(item))
            {
                return null;
            }

            return item;
        }

        private Item GetSiteStartItem(SiteInfo site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            if (string.IsNullOrEmpty(site.Database))
            {
                return null;
            }

            var database = Database.GetDatabase(site.Database);

            return database?.GetItem($"{site.RootPath}{site.StartItem}");
        }

        private SiteDefinition Create([NotNull] SiteInfo site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            return new SiteDefinition
            {
                Item = this.GetSiteRootItem(site),
                StartItem = new Lazy<Item>(() => this.GetSiteStartItem(site)),
                Name = site.Name,
                HostName = GetHostName(site),
                IsCurrent = this.IsCurrent(site),
                Site = site
            };
        }

        private Item GetSiteItemByHierarchy(Item item)
        {
            return item.Axes.GetAncestors().FirstOrDefault(IsSite);
        }

        private SiteDefinition GetSiteByHierarchy(Item item)
        {
            var siteItem = this.GetSiteItemByHierarchy(item);
            return siteItem == null ? null : this.SiteDefinitions.FirstOrDefault(s => s.Item.ID == siteItem.ID);
        }

        private bool IsCurrent(SiteInfo site)
        {
            return site != null && Context.Site != null && Context.Site.Name.Equals(site.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
