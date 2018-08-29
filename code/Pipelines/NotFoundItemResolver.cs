using System.IO;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Foundation.SitecoreExtensions.Extensions;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;

namespace Sitecore.Foundation.Multisite.Pipelines
{
    public class NotFoundItemResolver : HttpRequestProcessor
    {
        public const string NotFoundItemPropertyKey = "notFoundItem";

        public override void Process(HttpRequestArgs args)
        {
            if (RequestIsForPhysicalFile(args.Url.FilePath))
                return;

            if (args.LocalPath.StartsWith("/sitecore"))
                return;

            if (!HasNotFoundItemKey(Context.Site))
                return;

            if (IsValidContextItemResolved())
                return;

            Context.Item = GetSiteSpecificNotFoundItem();

            if (Context.Item == null)
                return;

            HttpContext.Current.Items[NotFoundItemPropertyKey] = true;
        }

        static bool RequestIsForPhysicalFile(string filePath)
            => File.Exists(HttpContext.Current.Server.MapPath(filePath));

        static bool IsValidContextItemResolved()
        {
            if (Context.Item == null || !Context.Item.HasContextLanguage())
                return false;

            return !(Context.Item.Visualization.Layout == null
              && string.IsNullOrEmpty(WebUtil.GetQueryString("sc_layout")));
        }

        static Item GetSiteSpecificNotFoundItem()
            => GetItemBySiteProperty(Context.Site, NotFoundItemPropertyKey);

        static Item GetItemByShortPath(Sites.SiteContext siteContext, string shortPath)
        {
            var fullPath = string.Concat(StringUtil.EnsurePostfix('/', siteContext.StartPath), shortPath.TrimStart('/'));

            return siteContext.Database.GetItem(fullPath);
        }

        static Item GetItemBySiteProperty(Sites.SiteContext siteContext, string propertyKey)
        {
            var property = siteContext.Properties[propertyKey];
            if (string.IsNullOrEmpty(property))
                return null;

            if (ID.IsID(property) || property.StartsWith(Constants.ContentPath))
                return siteContext.Database.GetItem(property);

            return GetItemByShortPath(siteContext, property);
        }

        static bool HasNotFoundItemKey(Sites.SiteContext siteContext)
        {
            return !string.IsNullOrEmpty(siteContext.Properties[NotFoundItemPropertyKey]);
        }
    }
}
