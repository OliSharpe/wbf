using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class RecordsManagementAdmin : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WBFarm farm = WBFarm.Local;
            WBTaxonomy subjectTagsTaxonomy = WBTaxonomy.GetSubjectTags(SPContext.Current.Site);

            WBSubjectTagsRecordsRoutings routings = farm.SubjectTagsRecordsRoutings(subjectTagsTaxonomy);

            if (!String.IsNullOrEmpty(Request.QueryString["RemoveIndex"]))
            {
                int index;
                if (Int32.TryParse(Request.QueryString["RemoveIndex"], out index))
                {
                    routings.RemoveAtIndex(index);

                    // Now save the new routing information:

                    SPContext.Current.Web.AllowUnsafeUpdates = true;
                    farm.SubjectTagsRecordsRoutingsString = routings.ToString();
                    farm.Update();
                    SPContext.Current.Web.AllowUnsafeUpdates = false;

                    SPUtility.Redirect("/_admin/WorkBoxFramework/RecordsManagementAdmin.aspx", SPRedirectFlags.Static, Context);
                    return;
                }

            }

            if (!IsPostBack)
            {
                PublicDocumentEmailAlertsTo.Text = farm.PublicDocumentEmailAlertsTo;
            }

            String html = "<table cellpadding='6'>\n";
            html += "<tr><th>Subject Tag</th><th>Public Library</th><th>Extranet Library</th></tr>\n\n";
            if (routings.Count > 0)
            {
                int index = 0;
                foreach (WBSubjectTagRecordsRoutings routing in routings)
                {
                    html += "<tr><td>" + routing.SubjectTag.FullPath + "</td><td>"
                        + routing.PublicDocumentsLibrary + "</td><td>"
                        + routing.ExtranetDocumentsLibrary + "</td><td>"
                        + "<a href='#' onclick='WorkBoxFramework_callDialog(\"/_admin/WorkBoxFramework/EditSubjectTagRouting.aspx?RouteIndex=" + index + "\");'>edit</a></td><td>"
                        + "<a href='#' onclick='if (window.confirm(\"Are you sure you want to remove routing?\")) { location.href = \"/_admin/WorkBoxFramework/RecordsManagementAdmin.aspx?RemoveIndex=" + index + "\"; }'>remove</a></td></tr>\n\n";
                    index++;
                }
            }
            else
            {
                html += "<tr><td colspan='5'><i>No subject tag routings</i></td></tr>\n\n";
            }

            html += "<tr><td colspan='5'><a href='#' onclick='WorkBoxFramework_callDialog(\"/_admin/WorkBoxFramework/EditSubjectTagRouting.aspx\");'>Add another subject tag routing rule</a></td></tr>\n\n";
            SubjectTagsRecordsRoutings.Text = html;

        }

        protected void CancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("/applications.aspx", SPRedirectFlags.Static, Context);
        }

        protected void SaveButton_OnClick(object sender, EventArgs e)
        {
            WBFarm farm = WBFarm.Local;

            farm.PublicDocumentEmailAlertsTo = PublicDocumentEmailAlertsTo.Text;

            farm.Update();

            SPUtility.Redirect("/applications.aspx", SPRedirectFlags.Static, Context);
        }

    }
}
