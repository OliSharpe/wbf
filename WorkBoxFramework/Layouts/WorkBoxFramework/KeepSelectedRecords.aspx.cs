using System;
using System.Web;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;


namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class KeepSelectedRecords : WorkBoxDialogPageBase
    {
        WBRecordsManager manager = null;
        String selectedRecordsString = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            WBLogging.Generic.Verbose("In Page_Load for the keeping of selected records");

            manager = new WBRecordsManager(SPContext.Current.Web.CurrentUser.LoginName);

            // If this is the initial call to the page then we need to load the basic details of the document we're publishing out:
            if (!IsPostBack)
            {
                selectedRecordsString = Request.QueryString["SelectedRecords"];

                SelectedRecords.Value = selectedRecordsString;

                RenderRecordsToKeep();
            }
            else
            {
                selectedRecordsString = SelectedRecords.Value;
            }
        }

        protected void RenderRecordsToKeep()
        {
            if (String.IsNullOrEmpty(selectedRecordsString))
            {
                RecordsBeingKept.Text = "<tr><td><i>No records were selected!</i></td></tr>";
                return;
            }

            String html = @"
<tr>
    <td class=""wbf-field-name-panel"">
        <div class=""wbf-field-name"">Records to Keep</div>
    </td>
    <td class=""wbf-field-value-panel"">
";

            String[] recordDetailsPairs = selectedRecordsString.Split('_');
            List<String> allRecordIDsToKeep = new List<String>();
            List<String> allRecordFilenamesToKeep = new List<String>();
            foreach (String recordDetailsPair in recordDetailsPairs)
            {
                String[] recordDetails = recordDetailsPair.Split('x');
                String recordSeriesID = recordDetails[0];
                String recordID = recordDetails[1];

                WBRecord versionRecord = manager.Libraries.GetRecordByID(recordID);

                String filename = versionRecord.Name;

                html += @"
                            <div>
            <img src='/_layouts/images/WorkBoxFramework/list-item-16.png' alt='Record to be kept'/>
            <img src='" + WBUtils.DocumentIcon16(filename) + "' alt='Icon for file " + filename + "'/> " + filename + @"
        </div>
";

                allRecordIDsToKeep.Add(versionRecord.RecordID);
                allRecordFilenamesToKeep.Add(versionRecord.Name);
            }

            html += @"
    </td>
</tr>
";

            RecordsBeingKept.Text = html;
            AllRecordIDsToKeep.Value = String.Join(",", allRecordIDsToKeep.ToArray());
            AllRecordFilenamesToKeep.Value = String.Join(",", allRecordFilenamesToKeep.ToArray());
        }

        protected void keepAllButton_OnClick(object sender, EventArgs e)
        {
            string redirectUrl = "WorkBoxFramework/ActuallyKeepRecords.aspx";
            string queryString = "AllRecordIDsToKeep=" + AllRecordIDsToKeep.Value;
            queryString += "&ReasonToKeepRecords=" + WBUtils.UrlDataEncode(KeepReason.Text);

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context, queryString);
        }


        protected void cancelButton_OnClick(object sender, EventArgs e)
        {
            returnFromDialogCancel("Keeping of records was cancelled");
        }

    }
}
