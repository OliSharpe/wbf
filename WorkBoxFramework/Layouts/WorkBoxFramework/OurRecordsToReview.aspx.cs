using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class OurRecordsToReview : LayoutsPageBase
    {
        WBRecordsManager manager = null;
        WorkBox workBox = null;
        WBTeam team = null;
        bool masterLibraryHasVersions = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            manager = new WBRecordsManager(SPContext.Current.Web.CurrentUser.LoginName);
            masterLibraryHasVersions = manager.Libraries.ProtectedMasterLibrary.List.EnableVersioning;

            if (WorkBox.IsWebAWorkBox(SPContext.Current.Web))
            {
                workBox = new WorkBox(SPContext.Current);
                team = workBox.OwningTeam;
            }
            else
            {
                team = WBTeam.GetFromTeamSite(SPContext.Current);
            }

            if (!IsPostBack && team != null)
            {
                WBQuery query = manager.GetQueryForTeamsPublicRecordsToReview(team);

                WBLogging.Debug("The query is: " + query.JustCAMLQuery(manager.Libraries.ProtectedMasterLibrary.Site));

                SPListItemCollection items = manager.Libraries.ProtectedMasterLibrary.List.WBxGetItems(manager.Libraries.ProtectedMasterLibrary.Site, query);
                RenderFoundRecords(items);
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (manager != null)
            {
                manager.Dispose();
                manager = null;
            }

            if (workBox != null)
            {
                workBox.Dispose();
                workBox = null;
            }
        }

        private void RenderFoundRecords(SPListItemCollection items)
        {
            if (items == null)
            {
                FoundRecords.Text = "<i>items was null!</i>";
                return;
            }

            if (items.Count == 0)
            {
                FoundRecords.Text = "<i>No records found</i>";
                return;
            }

            String html = "<table class='wbf-record-series-details'>\n";

            html += "<tr>"
+ "<th class='wbf-record-series-odd'></th>"
+ "<th class='wbf-record-series-odd'>Title</th>"
+ "<th class='wbf-record-series-even'>Filename</th>"
+ "<th class='wbf-record-series-odd'>Version</th>"
+ "<th class='wbf-record-series-even'>Protective Zone</th>"
+ "<th class='wbf-record-series-odd'>Published Date</th>"
+ "<th class='wbf-record-series-even'>Published By</th>"
+ "<th class='wbf-record-series-odd'>Review Date</th>"
+ "<th class='wbf-record-series-even'>Archive After</th>"
+ "<th class='wbf-record-series-even'></th>"
+ "</tr>\n";



            foreach (SPListItem item in items)
            {
                if (ItemCanBePicked(item))
                {
                    WBDocument document = new WBDocument(manager.Libraries.ProtectedMasterLibrary, item);

                    String publishedDateString = "";
                    if (document.Item.WBxHasValue(WBColumn.DatePublished))
                    {
                        publishedDateString = String.Format("{0:dd/MM/yyyy}", document[WBColumn.DatePublished]);
                    }
                    if (publishedDateString == "" && document.Item.WBxHasValue(WBColumn.Modified))
                    {
                        publishedDateString = String.Format("{0:dd/MM/yyyy}", document[WBColumn.Modified]);
                    }

                    String publishedByString = "<unknown>";
                    SPUser publishedBy = document.Item.WBxGetSingleUserColumn(WBColumn.PublishedBy);

                    if (publishedBy != null)
                    {
                        publishedByString = publishedBy.Name;
                    }
                    else
                    {
                        // If the published by column isn't set then we'll use the author column as a backup value:
                        publishedBy = document.Item.WBxGetSingleUserColumn(WBColumn.Author);
                        if (publishedBy != null)
                        {
                            publishedByString = publishedBy.Name;
                        }
                    }

                    long fileLength = (document.Item.File.Length / 1024);
                    if (fileLength == 0) fileLength = 1;
                    String fileLengthString = "" + fileLength + " KB";

                    String reviewDateString = "";
                    String archiveAfterString = "";
                    if (document.Item.WBxHasValue(WBColumn.ReviewDate))
                    {
                        reviewDateString = String.Format("{0:dd/MM/yyyy}", document[WBColumn.ReviewDate]);
                        archiveAfterString = String.Format("{0:dd/MM/yyyy}", ((DateTime)document[WBColumn.ReviewDate]).AddDays(28));
                    }

                    String version = document.RecordSeriesIssue;
                    if (masterLibraryHasVersions)
                    {
                        version += "." + (item.Versions.Count - 1).ToString();
                    }

                    html += "<tr>"
                        + "<td class='wbf-record-series-summary-detail'><input type='checkbox' class='wbf-our-records-check-boxes' data-record-id='" + document.RecordSeriesID + "x" + document.RecordID + "' onclick=\"WBF_checkbox_changed(event);\"/></td>"
                        + "<td class='wbf-record-series-summary-detail'>" + document.Title + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + document.Filename + "</td>"
                        + "<td class='wbf-record-series-summary-detail wbf-centre'>" + version + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + document.ProtectiveZone + "</td>"
                        + "<td class='wbf-record-series-summary-detail wbf-centre'>" + publishedDateString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + publishedByString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + reviewDateString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + archiveAfterString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'><a href='#' onclick='WorkBoxFramework_viewRecordSeriesDetails(\"" + document.RecordSeriesID + "\", \"" + document.RecordID + "\");'>view details</a></td>"
                        + "</tr>";
                }
            }


            html += "\n</table>";

            FoundRecords.Text = html;

            // This should attach the right function to the checkboxes
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AttachChangeListeners", "$(function () { WBF_add_checkbox_change_function(); });", true);            
        }

        public static SPListItemCollection GetRecordsInFolder(SPFolder folder)
        {
            SPList list = folder.ParentWeb.Lists[folder.ParentListId];
            SPQuery query = new SPQuery();
            query.Folder = folder;                        //set folder for seaching;
            query.ViewAttributes = "Scope=\"Recursive\""; //set recursive mode for items seaching;
            return list.GetItems(query);
        }

        private bool ItemCanBePicked(SPListItem item)
        {
            if (item == null) return false;

            if (String.IsNullOrEmpty(item.WBxGetAsString(WBColumn.RecordID))) return false;
            if (item.WBxGetAsString(WBColumn.LiveOrArchived) == WBColumn.LIVE_OR_ARCHIVED__ARCHIVED) return false;

            String recordSeriesStatus = item.WBxGetAsString(WBColumn.RecordSeriesStatus);
            if (recordSeriesStatus == "Latest" || String.IsNullOrEmpty(recordSeriesStatus)) return true;

            return false;
        }

    }
}
