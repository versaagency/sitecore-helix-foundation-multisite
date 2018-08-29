using Sitecore.Caching.Placeholders;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Foundation.Multisite.Placeholders
{
    using Sitecore.Sites;

    internal class SiteSpecificPlaceholderCache : PlaceholderCache
    {
        private ID itemRootId;

        public SiteSpecificPlaceholderCache(string databaseName, string siteName, PlaceholderCache fallbackCache)
            : base(databaseName)
        {
            this.FallbackCache = fallbackCache;
            this.Site = siteName == null ? null : Factory.GetSite(siteName);
        }

        public PlaceholderCache FallbackCache { get; }

        public SiteContext Site { get; }

        public override ID ItemRootId
        {
            get
            {
                if (!ID.IsNullOrEmpty(this.itemRootId))
                {
                    return this.itemRootId;
                }

                var siteRootId = this.GetItemRootIdFromSite();
                this.itemRootId = !ID.IsNullOrEmpty(siteRootId) ? siteRootId : base.ItemRootId;
                return this.itemRootId;
            }
        }

        public override Item this[string key]
        {
            get
            {
                var item = base[key];
                return item ?? this.GetPlaceholderItemFromFallbackCache(key);
            }
        }

        private Item GetPlaceholderItemFromFallbackCache(string key)
        {
            return this.FallbackCache?[key];
        }

        private ID GetItemRootIdFromSite()
        {
            var rootValue = this.Site?.Properties["placeholderSettingsRoot"];
            if (string.IsNullOrWhiteSpace(rootValue))
            {
                return null;
            }

            var rootItem = this.Database.GetItem(rootValue);
            return rootItem?.ID;
        }
    }
}
