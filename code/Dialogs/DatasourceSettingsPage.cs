using System;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.XmlControls;

namespace Sitecore.Foundation.Multisite.Dialogs
{
    public class DatasourceSettingsPage : DialogForm
    {
#pragma warning disable SA1306 // Field names must begin with lower-case letter
#pragma warning disable SA1401 // Fields must be private
        protected DataContext DataContext;
        protected XmlControl Dialog;
        protected Border Items;
        protected TreeviewEx Treeview;
#pragma warning restore SA1401 // Fields must be private
#pragma warning restore SA1306 // Field names must begin with lower-case letter

        private const string DialogRootSettingName = "Foundation.Multisite.DatasourceDialogRoot";

        protected string Root
            => Settings.GetSetting(DialogRootSettingName, "/sitecore/layout/renderings/feature");

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            if (this.DataContext != null)
            {
                this.DataContext.GetFromQueryString();
                this.DataContext.Root = this.Root;
                this.DataContext.Filter = this.GetFilter();
            }
        }

        protected void OK_Click()
        {
            var selectionItem = this.Treeview.GetSelectionItem();
            if (selectionItem == null)
            {
                SheerResponse.Alert("Select an item.");
            }
            else
            {
                this.SetDialogResult(selectionItem);
                SheerResponse.CloseWindow();
            }
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull(args, nameof(args));
            this.OK_Click();
        }

        protected virtual void SetDialogResult(Item selectedItem)
        {
            Assert.ArgumentNotNull(selectedItem, nameof(selectedItem));
            SheerResponse.SetDialogValue(selectedItem.ID.ToString());
        }

        protected string GetFilter()
        {
            return string.Format("(contains(@@templatekey, 'folder') or contains(@Datasource Location, 'site:'))");
        }
    }
}
