using Sitecore.Data.Items;

namespace Sitecore.Foundation.Multisite.Providers
{
    public interface ISiteSettingsProvider
    {
        Item GetSetting(Item contextItem, string settingsType, string setting);
    }
}
