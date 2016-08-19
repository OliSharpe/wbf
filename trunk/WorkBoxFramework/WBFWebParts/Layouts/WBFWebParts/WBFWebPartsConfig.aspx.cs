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

                SPSite site = SPContext.Current.Site;

                RecordsLibraryToUse.DataSource = WBFWebPartsUtils.GetRecordsLibraryOptions();
                RecordsLibraryToUse.DataBind();
                RecordsLibraryToUse.WBxSafeSetSelectedValue(WBFWebPartsUtils.GetRecordsLibraryToUse(site));
                LocalPublicLibraryURL.Text = WBFWebPartsUtils.GetLocalPublicLibraryURL(site);

                UseExtranetLibrary.Checked = WBFWebPartsUtils.UseExtranetLibrary(site);
                LocalExtranetLibraryURL.Text = WBFWebPartsUtils.GetLocalExtranetLibraryURL(site);

                ShowFileIcons.Checked = WBFWebPartsUtils.ShowFileIcons(site);
                ShowKBFileSize.Checked = WBFWebPartsUtils.ShowKBFileSize(site);
                ShowDescription.Checked = WBFWebPartsUtils.ShowDescription(site);
            }
        }


        protected void okButton_OnClick(object sender, EventArgs e)
        {
            SPSite site = SPContext.Current.Site;

            WBFWebPartsUtils.SetRecordsLibraryToUse(site, RecordsLibraryToUse.SelectedValue);
            WBFWebPartsUtils.SetLocalPublicLibraryURL(site, LocalPublicLibraryURL.Text);

            WBFWebPartsUtils.SetUseExtranetLibrary(site, UseExtranetLibrary.Checked);
            WBFWebPartsUtils.SetLocalExtranetLibraryURL(site, LocalExtranetLibraryURL.Text);

            WBFWebPartsUtils.SetShowFileIcons(site, ShowFileIcons.Checked);
            WBFWebPartsUtils.SetShowKBFileSize(site, ShowKBFileSize.Checked);
            WBFWebPartsUtils.SetShowDescription(site, ShowDescription.Checked);

            site.RootWeb.Update();

            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }

        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("settings.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        }
    }
}
