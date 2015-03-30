using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using WorkBoxFramework;

namespace WBFWebParts.Layouts.WBFWebParts
{
    public partial class WBFWebPartsConfig : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                WBFarm farm = WBFarm.Local;

                RecordsLibraryToUse.DataSource = WBFWebPartsUtils.GetRecordsLibraryOptions();
                RecordsLibraryToUse.DataBind();
                RecordsLibraryToUse.WBxSafeSetSelectedValue(WBFWebPartsUtils.GetRecordsLibraryToUse(SPContext.Current.Site));

                UseExtranetLibrary.Checked = WBFWebPartsUtils.UseExtranetLibrary(SPContext.Current.Site);

                ShowFileIcons.Checked = WBFWebPartsUtils.ShowFileIcons(SPContext.Current.Site);
                ShowKBFileSize.Checked = WBFWebPartsUtils.ShowKBFileSize(SPContext.Current.Site);
                ShowDescription.Checked = WBFWebPartsUtils.ShowDescription(SPContext.Current.Site);
            }
        }


        protected void okButton_OnClick(object sender, EventArgs e)
        {
            WBFWebPartsUtils.SetRecordsLibraryToUse(SPContext.Current.Site, RecordsLibraryToUse.SelectedValue);

            WBFWebPartsUtils.SetUseExtranetLibrary(SPContext.Current.Site, UseExtranetLibrary.Checked);

            WBFWebPartsUtils.SetShowFileIcons(SPContext.Current.Site, ShowFileIcons.Checked);
            WBFWebPartsUtils.SetShowKBFileSize(SPContext.Current.Site, ShowKBFileSize.Checked);
            WBFWebPartsUtils.SetShowDescription(SPContext.Current.Site, ShowDescription.Checked);

            SPContext.Current.Site.RootWeb.Update();

            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }
    }
}
