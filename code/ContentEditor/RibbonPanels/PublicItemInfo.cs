using System;
using System.Linq;
using System.Web.UI;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Links;
using Sitecore.Publishing;
using Sitecore.Resources.Media;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Sites;
using Sitecore.Web.UI.WebControls.Ribbons;

namespace Sitecore.Foundation.Multisite.ContentEditor.RibbonPanels
{
    public class PublicItemInfo : RibbonPanel
    {
        public override void Render(HtmlTextWriter output, Ribbon ribbon, Item button, CommandContext context)
        {
            // Validate Arguments
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNull(ribbon, nameof(ribbon));
            Assert.ArgumentNotNull(button, nameof(button));
            Assert.ArgumentNotNull(context, nameof(context));

            try
            {
                var htmlLinkOutput = string.Empty;
                var htmlPublishOutput = string.Empty;

                // Confirm item exists in the context
                var contextItem = context.Items.FirstOrDefault();
                if (contextItem != null)
                {
                    var url = string.Empty;

                    if (contextItem.Paths.IsContentItem)
                    {
                        // Find the best matching site
                        var contextItemPath = contextItem.Paths.Path.ToLower();
                        var site = SiteContextFactory.Sites
                            .FirstOrDefault(s => s.VirtualFolder == "/" && s.RootPath != string.Empty && contextItemPath.StartsWith(FileUtil.MakePath(s.RootPath, s.StartItem).ToLower()));

                        if (site != null)
                        {
                            var urlOptions = LinkManager.GetDefaultUrlOptions();
                            urlOptions.Site = new Sites.SiteContext(site);
                            url = LinkManager.GetItemUrl(contextItem, urlOptions);
                        }
                    }
                    else if (contextItem.Paths.IsMediaItem && contextItem.TemplateID != TemplateIDs.MediaFolder)
                    {
                        using (new SiteContextSwitcher(Factory.GetSite("website")))
                            url = MediaManager.GetMediaUrl(contextItem);
                    }

                    // Create the HTML output to render.
                    if (!string.IsNullOrEmpty(url))
                    {
                        htmlLinkOutput = string.Format("<div style='padding:3px 3px 5px 7px;display: inline-block;'><div style='padding:3px 0px 5px 0px;'>Link to this page</div><div style='font-weight:bold'><a href='{0}' target='_blank' style='color:blue;text-decoration:underline'>{0}</a></div></div>&nbsp;&nbsp;", url);
                    }

                    // Determine if the item is published

                    // Obtain reference to the master database
                    var masterDb = Database.GetDatabase("master");

                    // Find all of the publishing targets and determine if the item has been published to those targets
                    var publishingTargets = PublishManager.GetPublishingTargets(masterDb);
                    foreach (var publishingTarget in publishingTargets)
                    {
                        var targetDatabaseName = publishingTarget.Fields["Target database"].Value;
                        var isPreviewTarget = publishingTarget.Fields["Preview publishing target"].Value == "1";
                        if (!string.IsNullOrEmpty(targetDatabaseName) && !isPreviewTarget)
                        {
                            var targetDatabase = Database.GetDatabase(targetDatabaseName);
                            if (targetDatabase != null)
                            {
                                // SelectSingleItem does a direct request to the database for the item
                                var item = targetDatabase.SelectSingleItem(contextItem.ID.ToString());
                                if (item != null)
                                {
                                    foreach (var language in item.Languages)
                                    {
                                        var languageVersion = item.Versions.GetLatestVersion(language);
                                        if (languageVersion != null && languageVersion.Versions.Count > 0)
                                        {
                                            htmlPublishOutput += string.Format("<div>{0} - {1}</div>", languageVersion.Version.Number, languageVersion.Language.CultureInfo.DisplayName);
                                        }
                                    }
                                }

                                // Exit after the first publishing target
                                break;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(htmlPublishOutput))
                        htmlPublishOutput = string.Format("<div>{0}</div>", "No");

                    htmlPublishOutput = string.Format("<div style='padding:3px 3px 5px 7px;display: inline-block;'><div style='padding:3px 0px 5px 0px;font-weight:bold'>Published to Web</div>{0}</div>", htmlPublishOutput);
                }

                var htmlOutput =
                                string.Format(
                                    "<div class='scRibbonToolbarText' style='padding:0 10px 7px 5px;height:auto;border:none !important;float:none;display:inline-block;'>" +
                                       "{0}" +
                                       "{1}" +
                                    "</div>",
                                    htmlLinkOutput,
                                    htmlPublishOutput);

                output.Write(htmlOutput);
            }
            catch (Exception ex)
            {
                Log.Error("Exception in custom ItemUrlInfo Ribbon: " + ex.Message, this);
            }
        }
    }
}
