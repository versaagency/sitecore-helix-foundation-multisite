using Sitecore.Data.Items;

namespace Sitecore.Foundation.Multisite.Providers
{
    public interface IDatasourceProvider
    {
        Item[] GetDatasourceLocations(Item contextItem, string name);

        Item GetDatasourceTemplate(Item contextItem, string name);
    }
}
