using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ViewRecordSeriesDetails : WorkBoxDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String html = "<table class='wbf-record-series-details'>\n";

            html += "<tr>"
    + "<th class='wbf-record-series-odd'>Version</th>"
    + "<th class='wbf-record-series-even'>Published Date</th>"
    + "<th class='wbf-record-series-odd'>Published By</th>"
    + "<th class='wbf-record-series-even'>Status</th>"
    + "<th class='wbf-record-series-odd'>File Size</th>"
    + "<th class='wbf-record-series-odd'></th>"
    + "</tr>\n";

            
            
            String recordSeriesID = Request.QueryString["RecordSeriesID"];

            using (WBRecordsManager manager = new WBRecordsManager())
            {
                WBRecordsLibrary masterLibrary = manager.Libraries.ProtectedMasterLibrary;
                SPList masterLibraryList = masterLibrary.List;
                WBQuery query = new WBQuery();
                query.AddEqualsFilter(WBColumn.RecordSeriesID, recordSeriesID);
                query.OrderBy(WBColumn.RecordSeriesIssue, false);

                SPListItemCollection items = masterLibraryList.WBxGetItems(SPContext.Current.Site, query);

                /*
                List<WBDocument> versions = new List<WBDocument>();
                foreach (SPListItem item in items)
                {

                    bool notInserted = true;
                    for (int i = 0; i < versions.Count && notInserted; i++)
                    {
                        

                        if (document.RecordSeriesIssue.WBxToInt() > versions[i].RecordSeriesIssue.WBxToInt())
                    }

                }
                 * */


                foreach (SPListItem item in items)
                {
                    WBDocument document = new WBDocument(masterLibrary, item);
                    WBRecord record = new WBRecord(masterLibrary.Libraries, item);

                    String publishedDateString = "";
                    if (document.Item.WBxHasValue(WBColumn.DatePublished)) {
                       publishedDateString = String.Format("{0:MM/dd/yyyy}", document[WBColumn.DatePublished]);
                    }
                    if (publishedDateString == "" && document.Item.WBxHasValue(WBColumn.Modified))
                    {
                        publishedDateString = String.Format("{0:MM/dd/yyyy}", document[WBColumn.Modified]);
                    }

                    String publishedByString = document.Item["Author"].WBxToString();

                    SPUser publishedBy = document.Item.WBxGetSingleUserColumn(WBColumn.PublishedBy);

                    if (publishedBy != null)
                    {
                        publishedByString = publishedBy.Name;
                    }

                    String approvedByString = ""; 

                    List<SPUser> approvedBy = document.Item.WBxGetMultiUserColumn(WBColumn.PublishingApprovedBy);

                    if (approvedBy != null && approvedBy.Count > 0)
                    {
                        foreach (SPUser approver in approvedBy)
                        {
                            approvedByString = approvedByString + approver.Name + "; ";
                        }
                    }


                    long fileLength = (document.Item.File.Length / 1024);
                    if (fileLength == 0) fileLength = 1;
                    String fileLengthString = "" + fileLength + " KB";

                    
                    String status = document[WBColumn.RecordSeriesStatus].WBxToString();
                    if (String.IsNullOrEmpty(status)) status = "Latest";

                    html += "<tr>"
                        + "<td class='wbf-record-series-summary-issue'>" + document.RecordSeriesIssue + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + publishedDateString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + publishedByString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + status + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + fileLengthString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'><a href='#' onclick='revealRecordID(\"" + document.RecordID + "\");'>more &gt;</a></td>"
                        + "</tr>\n";

                    html += "<tr class='wbf-record-details' data-record-id='" + document.RecordID + "' style=' display: none;' ><td colspan=6 class='wbf-record-series-details-panel-cell'><table class='wbf-record-series-details-panel'>";

                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Title</td><td class='wbf-record-series-detail-odd'>" + document.Title + "</td></tr>";
                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Filename</td><td class='wbf-record-series-detail-even'>" + document.Filename + "</td></tr>";
                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Location</td><td class='wbf-record-series-detail-odd'>" + document.LibraryLocation + "</td></tr>";
                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Subject Tags</td><td class='wbf-record-series-detail-even'>" + document.SubjectTags.Names() + "</td></tr>";
                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Owning Team</td><td class='wbf-record-series-detail-odd'>" + document.OwningTeam.Name + "</td></tr>";
                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Involved Teams</td><td class='wbf-record-series-detail-even'>" + document.InvolvedTeams.Names() + "</td></tr>";
                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Protective Zone</td><td class='wbf-record-series-detail-odd'>" + document.ProtectiveZone + "</td></tr>";
                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Status</td><td class='wbf-record-series-detail-even'>" + status + "</td></tr>";

                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Approved By</td><td class='wbf-record-series-detail-even'>" + approvedByString + "</td></tr>";
                    html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Approval Statement</td><td class='wbf-record-series-detail-even'>" + document[WBColumn.PublishingApprovalStatement].WBxToString() + "</td></tr>";

                    html += "</table>\n";
                }

            }

            html += "</table>";

            ViewRecordSeriesTable.Text = html;
        }

        protected void closeButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogWithOK();
        }

    }
}
