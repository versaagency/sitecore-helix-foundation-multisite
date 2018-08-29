using System.Text.RegularExpressions;

namespace Sitecore.Foundation.Multisite.Providers
{
    public static class DatasourceConfigurationService
    {
        public const string SiteDatasourceMatchPattern = @"^site:(\w*)$";

        public static string GetSiteDatasourceConfigurationName(string datasourceLocationValue)
        {
            var match = Regex.Match(datasourceLocationValue, SiteDatasourceMatchPattern);
            return !match.Success ? null : match.Groups[1].Value;
        }

        public static bool IsSiteDatasourceLocation(string datasourceLocationValue)
        {
            var match = Regex.Match(datasourceLocationValue, SiteDatasourceMatchPattern);
            return match.Success;
        }
    }
}
