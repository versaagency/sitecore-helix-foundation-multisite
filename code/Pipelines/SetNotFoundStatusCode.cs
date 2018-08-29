using System.Net;
using System.Web;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.Foundation.Multisite.Pipelines
{
    public class SetNotFoundStatusCode : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if (HttpContext.Current.Items[NotFoundItemResolver.NotFoundItemPropertyKey] != null &&
                (bool)HttpContext.Current.Items[NotFoundItemResolver.NotFoundItemPropertyKey])
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}
