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
    + "<th class='wbf-record-series-even'>Filename</th>"
    + "<th class='wbf-record-series-odd'>Published</th>"
    + "<th class='wbf-record-series-even'>Published By</th>"
    + "<th class='wbf-record-series-odd'>Modified</th>"
    + "<th class='wbf-record-series-even'>Status</th>"
    + "<th class='wbf-record-series-odd'>File Size</th>"
    + "<th class='wbf-record-series-odd'></th>"
    + "</tr>\n";

            
            
            String recordSeriesID = Request.QueryString["RecordSeriesID"];

            using (WBRecordsManager manager = new WBRecordsManager(SPContext.Current.Web.CurrentUser.LoginName))
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

                Dictionary<String, String> checklistTextMap = manager.GetChecklistTextMap();



                if (masterLibrary.List.EnableVersioning)
                {
                    foreach (SPListItem item in items)
                    {
                        SPListItemVersionCollection versionCollection = item.Versions;

                        int versionCount = item.Versions.Count;

                        WBLogging.Debug("Item versions count | File versions count: " + versionCount + " | " + item.File.Versions.Count);

                        for (int i = 0; i < versionCount; i++)
                        {
                            SPListItemVersion version = versionCollection[i];
                            WBDocument document = new WBDocument(masterLibrary, version);
                            // We're going to render the minor version numbers counting up - even though lower index values are for more recent versions:
                            html += RenderHTMLForOneDocumentVersion(checklistTextMap, document, document.RecordSeriesIssue, (versionCount - 1 - i).ToString(), i);
                        }
                    }
                }
                else
                {
                    foreach (SPListItem item in items)
                    {
                        WBDocument document = new WBDocument(masterLibrary, item);
                        html += RenderHTMLForOneDocumentVersion(checklistTextMap, document, document.RecordSeriesIssue, null, -1);
                    }
                }


            }

            html += "</table>";

            ViewRecordSeriesTable.Text = html;
        }

        internal String RenderHTMLForOneDocumentVersion(Dictionary<String, String> checklistTextMap, WBDocument document, String majorVersion, String minorVersion, int minorVersionIndex)
        {
            String html = "";

            String versionNumber = majorVersion;
            if (!String.IsNullOrEmpty(minorVersion)) versionNumber += "." + minorVersion;

            String versionAsToggleID = versionNumber.Replace(".", "-");

            String publishedDateString = "";
            if (document.HasValue(WBColumn.DatePublished))
            {
                publishedDateString = String.Format("{0:dd/MM/yyyy}", document[WBColumn.DatePublished]);
            }
            if (publishedDateString == "" && document.HasValue(WBColumn.Modified))
            {
                publishedDateString = String.Format("{0:dd/MM/yyyy}", document[WBColumn.Modified]);
            }

            String publishedByString = "<unknown>";
            SPUser publishedBy = document.GetSingleUserColumn(WBColumn.PublishedBy);

            if (publishedBy != null)
            {
                publishedByString = publishedBy.Name;
            }
            else
            {
                // If the published by column isn't set then we'll use the author column as a backup value:
                publishedBy = document.GetSingleUserColumn(WBColumn.Author);
                if (publishedBy != null)
                {
                    publishedByString = publishedBy.Name;
                }
            }

            String modifiedByString = "";
            String modifiedOnString = "";

            if (document.IsSPListItemVersion)
            {
                SPFieldUserValue versionCreated = document.ItemVersion.CreatedBy;
                SPUser modifiedBy = versionCreated.User;
                if (modifiedBy != null)
                {
                    WBLogging.Debug("Version created by lookup ID: " + versionCreated.LookupId);
                    WBLogging.Debug("Version created by lookup Value: " + versionCreated.LookupValue);
                    WBLogging.Debug("Version created by as string: " + versionCreated.ToString());
                    WBLogging.Debug("Version created by SPUser.Login: " + modifiedBy.LoginName);
                    WBLogging.Debug("Version created by SPUser.Name: " + modifiedBy.Name);
                    modifiedByString = modifiedBy.Name;
                }

                modifiedOnString = String.Format("{0:dd/MM/yyyy}", document.ItemVersion.Created);
            }
            else
            {
                SPUser modifiedBy = document.GetSingleUserColumn(WBColumn.ModifiedBy);
                if (modifiedBy != null)
                {
                    modifiedByString = modifiedBy.Name;
                }
                if (document.HasValue(WBColumn.Modified))
                {
                    modifiedOnString = String.Format("{0:dd/MM/yyyy}", document[WBColumn.Modified]);
                }
            }


            String approvedByString = document.GetMultiUserColumn(WBColumn.PublishingApprovedBy).WBxToPrettyString();
            String iaoString = document.GetSingleUserColumn(WBColumn.IAOAtTimeOfPublishing).WBxToPrettyString();

            long fileLength = (document.Item.File.Length / 1024);
            if (fileLength == 0) fileLength = 1;
            String fileLengthString = "" + fileLength + " KB";

            String status = document[WBColumn.RecordSeriesStatus].WBxToString();
            String extraStatusCSS = "";
            if (String.IsNullOrEmpty(status)) status = "Latest";
            if (minorVersionIndex > 0)
            {
                status = "(old metadata)";
                extraStatusCSS = " wbf-old-metadata";
            }

            String explainStatus = "";
            if (status == "Latest")
            {
                if (document.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC)
                {
                    explainStatus = "(live on the public website)";
                }
                else if (document.ProtectiveZone == WBRecordsType.PROTECTIVE_ZONE__PUBLIC_EXTRANET)
                {
                    explainStatus = "(live on a public extranet website)";
                }
                else
                {
                    explainStatus = "(live on izzi intranet)";
                }
            }
            else if (status == "Retired")
            {
                explainStatus = "(visible on izzi intranet searches)";
            }
            else if (status == "Archived")
            {
                explainStatus = "(archived in the protected, master records library)";
            }

            String reviewDateString = "";
            if (document.HasValue(WBColumn.ReviewDate))
            {
                reviewDateString = String.Format("{0:dd/MM/yyyy}", document[WBColumn.ReviewDate]);
            }

            String checkInComments = "";
            if (document.IsSPListItemVersion)
            {
                int fileVersionsCount = document.Item.File.Versions.Count;

                WBLogging.Debug("File versions count: " + fileVersionsCount);
                WBLogging.Debug("minorVersionIndex = " + minorVersionIndex);

                int minorFileVersionIndex = fileVersionsCount - minorVersionIndex;
                WBLogging.Debug("minorFileVersionIndex = " + minorFileVersionIndex);

                if (minorFileVersionIndex >= 0 && minorFileVersionIndex < fileVersionsCount)
                {
                    SPFileVersion fileVersion = document.Item.File.Versions[minorFileVersionIndex];
                    checkInComments = fileVersion.CheckInComment;
                }
                else
                {
                    checkInComments = document.Item.File.CheckInComment;
                }
            }
            else
            {
                checkInComments = document.Item.File.CheckInComment;
            }


            String checklistCount = "0";
            String checklistDiv = "";

            String checklistCodes = document[WBColumn.PublishingApprovalChecklist].WBxToString();
            if (!String.IsNullOrEmpty(checklistCodes)) {
                String[] codes = checklistCodes.Split(';');
                checklistDiv = "<div id='wbf-checklist-" + versionAsToggleID + "' style='display: none;'>";
                foreach (String code in codes)
                {
                    checklistDiv += "<input type='checkbox' enabled='false' checked disabled/>" + checklistTextMap[code] + "<br/>";
                }
                checklistDiv += "</div>";

                checklistCount = codes.Length.ToString();
            }
            
            String issueOddOrEven = "odd";
            if (majorVersion.WBxToInt() % 2 == 0) issueOddOrEven = "even";

            html += "<tr>"
                + "<td class='wbf-record-series-summary-issue-" + issueOddOrEven + "'>" + versionNumber + "</td>"
                + "<td class='wbf-record-series-summary-detail wbf-record-series-detail-left'>" + document.Name + "</td>"
                + "<td class='wbf-record-series-summary-detail'>" + publishedDateString + "</td>"
                + "<td class='wbf-record-series-summary-detail'>" + publishedByString + "</td>"
                + "<td class='wbf-record-series-summary-detail'>" + modifiedOnString + "</td>"
                + "<td class='wbf-record-series-summary-detail " + extraStatusCSS + "'>" + status + "</td>"
                + "<td class='wbf-record-series-summary-detail wbf-record-series-detail-right'>" + fileLengthString + "</td>"
                + "<td class='wbf-record-series-summary-detail'><a href='#' class='wbf-more-or-less' id='wbf-more-or-less-" + versionAsToggleID + "' onclick='WBF_toggleByID(\"" + versionAsToggleID + "\");'><nobr>more &gt;</nobr></a></td>"
                + "</tr>\n";

            html += "<tr class='wbf-record-details' id='wbf-record-details-" + versionAsToggleID + "' style=' display: none;' ><td></td><td colspan='6' class='wbf-record-series-details-panel-cell'><table class='wbf-record-series-details-panel' width='100%'>";

            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Title</td><td class='wbf-record-series-detail-odd'>" + document.Title + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Filename</td><td class='wbf-record-series-detail-even wbf-record-series-detail-left'>" + document.Filename + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Location</td><td class='wbf-record-series-detail-odd'>" + document.LibraryLocation + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Subject Tags</td><td class='wbf-record-series-detail-even'>" + document.SubjectTags.Names() + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Owning Team</td><td class='wbf-record-series-detail-odd'>" + document.OwningTeam.Name + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Involved Teams</td><td class='wbf-record-series-detail-even'>" + document.InvolvedTeamsWithoutOwningTeam.Names() + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Protective Zone</td><td class='wbf-record-series-detail-odd'>" + document.ProtectiveZone + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Status</td><td class='wbf-record-series-detail-even'>" + status + " " + explainStatus + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Approved By</td><td class='wbf-record-series-detail-odd'>" + approvedByString + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Approval Checklist</td><td class='wbf-record-series-detail-even'><a href='#' onclick='WBF_toggleChecklist(\"" + versionAsToggleID + "\");'>" + checklistCount + " checklist items were ticked</a>" + checklistDiv + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>IAO When Published</td><td class='wbf-record-series-detail-odd'>" + iaoString + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Intended Web Page URL</td><td class='wbf-record-series-detail-even'>" + document[WBColumn.IntendedWebPageURL].WBxToString() + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Review Date</td><td class='wbf-record-series-detail-odd'>" + reviewDateString + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Modified By</td><td class='wbf-record-series-detail-even'>" + modifiedByString + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-odd'>Modified On</td><td class='wbf-record-series-detail-odd'>" + modifiedOnString + "</td></tr>";
            html += "<tr><td class='wbf-record-series-detail-title wbf-record-series-detail-even'>Reason for Change</td><td class='wbf-record-series-detail-even'>" + checkInComments + "</td></tr>";

            html += "<tr><td class='wbf-record-series-detail-even' colspan='2' align='center'><input type='button' value='View Document' onclick='window.open(\"" + document.AbsoluteURL + "\", \"_blank\");' />";
            if (minorVersionIndex <= 0 && status != "Archived")
            {
                html += "&nbsp;<input type='button' value='Edit Metadata' onclick='WBF_edit_records_metadata(\"" + document.RecordID + "\");'/>";
            }
            html += "</td></tr>";

            html += "</table>\n";

            return html;
        }

        protected void closeButton_OnClick(object sender, EventArgs e)
        {
            CloseDialogWithOK();
        }

    }
}
