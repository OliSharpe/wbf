using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.SearchOrBrowseOurRecords
{
    public partial class SearchOrBrowseOurRecordsUserControl : UserControl
    {
        WBRecordsManager manager = null;
        WorkBox workBox = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            manager = new WBRecordsManager();
            workBox = new WorkBox(SPContext.Current);


            if (!IsPostBack)
            {

                WBTermCollection<WBTerm> functionalAreas = workBox.OwningTeam.FunctionalArea(workBox.FunctionalAreasTaxonomy);
                TreeViewLocationCollection collection = new TreeViewLocationCollection(manager, "Browse Folders", "", functionalAreas);

                RecordsLibraryFolders.DataSource = collection;
                RecordsLibraryFolders.DataBind();

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



        protected void DoSearch_Click(object sender, EventArgs e)
        {
            string strQuery = "";
            strQuery = "<OrderBy><FieldRef Name='Title' Ascending='FALSE' /></OrderBy>";
            List<string> conditions = new List<string>();

            String filter = "";

            String searchText = SearchBox.Text;

            if (searchText != "")
            {
                filter = "<Or><Contains><FieldRef Name='BaseName'/><Value Type='Text'>" + searchText + "</Value></Contains><Contains><FieldRef Name='Title'/><Value Type='Text'>" + searchText + "</Value></Contains></Or>";
            }


            if (!String.IsNullOrEmpty(filter))
            {
                strQuery = strQuery + "<Where>" + filter + "</Where>";
            }

            SPList List = manager.Libraries.ProtectedMasterLibrary.List;
            SPQuery query = new SPQuery();

            query.Query = string.Format(strQuery);

            WBLogging.Debug("The query filter being used: \n" + query.Query);

            SPFolder protectedLibraryRootFolder = manager.Libraries.ProtectedMasterLibrary.List.RootFolder;

            WBTerm functionalArea = workBox.OwningTeam.FunctionalArea(workBox.FunctionalAreasTaxonomy)[0];

            WBLogging.Debug("Looking for folder: \n" + functionalArea.Name);

            SPFolder functionalAreaFolder = protectedLibraryRootFolder.WBxGetFolderPath(functionalArea.Name);

            if (functionalAreaFolder == null) WBLogging.Debug("functionalAreaFolder == null");
            else
            {
                WBLogging.Debug("Adding folder filter to query of: " + functionalAreaFolder.Name);
            //    query.Folder = functionalAreaFolder;
            }



            SPListItemCollection items = List.Items; //  GetItems(query);

            WBLogging.Debug("Found items: " + items.Count);

            RenderFoundRecords(items);

        }

        private String GetSelectedFolderPath()
        {
            string selectedPath = RecordsLibraryFolders.SelectedNode.ValuePath;
            if (string.IsNullOrEmpty(selectedPath)) selectedPath = "/";
            return selectedPath;
        }

        protected void RecordsLibraryFolders_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (RecordsLibraryFolders.SelectedNode != null)
            {

                // SelectedFolderPath.Text = selectedPath;

                // Now for the bit where the path is analysed to pick out the selected functional area and the records type:

                String selectedPath = GetSelectedFolderPath();

                String[] pathSteps = selectedPath.Split('/');

                // We're only interested in selections of 3rd level 'folders' that are: functional area / records type / records type  ... or below.
                if (pathSteps.Length < 3) return;

                WBTerm functionalArea = manager.FunctionalAreasTaxonomy.GetSelectedWBTermByPath(pathSteps[0]);
                if (functionalArea == null)
                {
                    WBLogging.Debug("The functional area part of the selected path came back null: " + selectedPath);
                    return;
                }

                Term recordsTypeTerm = manager.RecordsTypesTaxonomy.GetOrCreateSelectedTermByPath(pathSteps[1] + "/" + pathSteps[2]);
                if (recordsTypeTerm == null)
                {
                    WBLogging.Debug("The records type part of the selected path came back null: " + selectedPath);
                    return;
                }
                WBRecordsType recordsType = new WBRecordsType(manager.RecordsTypesTaxonomy, recordsTypeTerm);

                RenderRecordsLibraryFoldersSelection();
            }
        }

        private void RenderRecordsLibraryFoldersSelection()
        {
            SPFolder protectedLibraryRootFolder = manager.Libraries.ProtectedMasterLibrary.List.RootFolder;

            SPFolder recordsTypeFolder = protectedLibraryRootFolder.WBxGetFolderPath(GetSelectedFolderPath());

            SPListItemCollection items = GetRecordsInFolder(recordsTypeFolder);

            RenderFoundRecords(items);
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
                        publishedDateString = String.Format("{0:MM/dd/yyyy}", document[WBColumn.DatePublished]);
                    }
                    if (publishedDateString == "" && document.Item.WBxHasValue(WBColumn.Modified))
                    {
                        publishedDateString = String.Format("{0:MM/dd/yyyy}", document[WBColumn.Modified]);
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


                    html += "<tr>"
                        + "<td class='wbf-record-series-summary-detail'><input type='checkbox' class='wbf-our-records-check-boxes' data-record-id='" + document.RecordID + "' onclick=\"WBF_checkbox_changed(event);\"/></td>"
                        + "<td class='wbf-record-series-summary-detail'>" + document.Title + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + document.Filename + "</td>"
                        + "<td class='wbf-record-series-summary-detail wbf-centre'>" + document.RecordSeriesIssue + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + document.ProtectiveZone + "</td>"
                        + "<td class='wbf-record-series-summary-detail wbf-centre'>" + publishedDateString + "</td>"
                        + "<td class='wbf-record-series-summary-detail'>" + publishedByString + "</td>"
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
