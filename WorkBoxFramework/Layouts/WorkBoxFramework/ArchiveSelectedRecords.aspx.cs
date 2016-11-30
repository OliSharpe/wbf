using System;
using System.Web;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ArchiveSelectedRecords : WorkBoxDialogPageBase
    {
        WBRecordsManager manager = null;
        String selectedRecordsString = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            WBLogging.Generic.Verbose("In Page_Load for the archival of selected records");

            manager = new WBRecordsManager(SPContext.Current.Web.CurrentUser.LoginName);

            // If this is the initial call to the page then we need to load the basic details of the document we're publishing out:
            if (!IsPostBack)
            {
                selectedRecordsString = Request.QueryString["SelectedRecords"];

                SelectedRecords.Value = selectedRecordsString;

                RenderRecordsToArchive();
            }
            else
            {
                selectedRecordsString = SelectedRecords.Value;
            }
        }

        protected void RenderRecordsToArchive()
        {
            if (String.IsNullOrEmpty(selectedRecordsString))
            {
                RecordsBeingArchived.Text = "<tr><td><i>No records were selected!</i></td></tr>";
                return;
            }

            String html = @"
<tr>
    <td class=""wbf-field-name-panel"">
        <div class=""wbf-field-name"">Records to Archive</div>
    </td>
    <td class=""wbf-field-value-panel"">
";

            String[] recordDetailsPairs = selectedRecordsString.Split('_');
            List<String> allRecordIDsToArchive = new List<String>();
            List<String> allRecordFilenamesToArchive = new List<String>();
            foreach (String recordDetailsPair in recordDetailsPairs)
            {
                String[] recordDetails = recordDetailsPair.Split('x');
                String recordSeriesID = recordDetails[0];
                String recordID = recordDetails[1];

                SPListItemCollection versions = manager.Libraries.ProtectedMasterLibrary.GetLiveVersionsUpTo(recordSeriesID, recordID);
                html += GetHTMLForOneRecordVersion(versions);

                foreach (SPListItem version in versions)
                {
                    allRecordIDsToArchive.Add(version.WBxGetAsString(WBColumn.RecordID));
                    allRecordFilenamesToArchive.Add(version.Name);
                }
            }

            html += @"
    </td>
</tr>
";

            RecordsBeingArchived.Text = html;
            AllRecordIDsToArchive.Value = String.Join(",", allRecordIDsToArchive.ToArray());
            AllRecordFilenamesToArchive.Value = String.Join(",", allRecordFilenamesToArchive.ToArray());
        }

        public String GetHTMLForOneRecordVersion(SPListItemCollection items)
        {
            String html = "";

                foreach (SPListItem item in items)
                {
                    String filename = item.Name;

                    html += @"
                            <div>
            <img src='/_layouts/images/WorkBoxFramework/list-item-16.png' alt='Record to be archived'/>
            <img src='" + WBUtils.DocumentIcon16(filename) + "' alt='Icon for file " + filename + "'/> " + filename + @"
        </div>
";
                }

            return html;
        }

        protected void archiveAllButton_OnClick(object sender, EventArgs e)
        {
            string redirectUrl = "WorkBoxFramework/ActuallyArchiveRecords.aspx";
            string queryString = "AllRecordIDsToArchive=" + AllRecordIDsToArchive.Value;
            queryString += "&ReasonToArchiveRecords=" + WBUtils.UrlDataEncode(ArchiveReason.Text);

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context, queryString);
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Archiving of records was cancelled");
        }

    }
}
